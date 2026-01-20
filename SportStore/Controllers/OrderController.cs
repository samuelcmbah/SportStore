using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportStore.Dtos;
using SportStore.Models;
using SportStore.Models.Enums;
using SportStore.Services;
using SportStore.Services.IServices;
using SportStore.Utils;
using SportStore.ViewModels;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;

namespace SportStore.Controllers
{
    [AllowAnonymous]
    public class OrderController : Controller
    {
        private readonly ICartService cartService;
        private readonly SessionCart sessionCart;
        private readonly ICartDomainService cartDomainService;
        private readonly IOrderDomainService orderDomainService;
        private readonly IOrderRepository orderRepository;
        private readonly IOrderNotificationService orderNotificationService;
        private readonly IHttpClientFactory httpClientFactory;

        public OrderController(
            ICartService cartService,
            SessionCart sessionCart,
            ICartDomainService cartDomainService,
            IOrderDomainService orderDomainService,
            IOrderRepository orderRepository,
            IOrderNotificationService orderNotificationService,
            IHttpClientFactory httpClientFactory )
        {
            this.cartService = cartService;
            this.sessionCart = sessionCart;
            this.cartDomainService = cartDomainService;
            this.orderDomainService = orderDomainService;
            this.orderRepository = orderRepository;
            this.orderNotificationService = orderNotificationService;
            this.httpClientFactory = httpClientFactory;
        }

        private async Task<Cart> GetCartAsync()
        {
            if (User.Identity!.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                return await cartService.GetOrCreateCartByUserIdAsync(userId);
            }

            return sessionCart.GetCart();
        }

        [HttpGet]
        [Route("Order")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            
            var orders = await orderRepository.GetOrdersByUserAsync(userId);
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = await GetCartAsync();

            if (!cart.CartItems.Any())
            {
                return RedirectToAction("ViewCart", "Cart");
            }

            var vm = new CheckoutViewModel
            {
                TotalPrice = cartDomainService.GetTotalPrice(cart)
            };

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel vm)
        {
            var cart = await GetCartAsync();

            if (!cart.CartItems.Any())
            {
                ModelState.AddModelError("", "Your cart is empty.");
            }

            if (!ModelState.IsValid)
            {
                vm.TotalPrice = cartDomainService.GetTotalPrice(cart);
                return View(vm);
            }

            var userId = User.Identity!.IsAuthenticated
                ? User.FindFirstValue(ClaimTypes.NameIdentifier)
                : null;

            // ✅ Get user email from claims
            var userEmail = User.Identity!.IsAuthenticated
                ? User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(ClaimTypes.Name)
                : "guest@sportstore.com";

            var order = orderDomainService.CreateOrderFromCart(cart, vm, userId);

            await orderRepository.CreateOrderAsync(order);

            var payload = new
            {
                ExternalUserId = userEmail, // 
                Amount = cartDomainService.GetTotalPrice(cart),
                Purpose = 0,  // ProductCheckout
                Provider = 0, // Paystack  
                AppName = "SportStore",
                ExternalReference = order.OrderID.ToString(),
                RedirectUrl = Url.Action("Completed", "Order", new { orderId = order.OrderID }, Request.Scheme),
                NotificationUrl = "https://localhost:7001/api/notifications/paybridge"
            };
            
            var json = JsonSerializer.Serialize(payload);
            Console.WriteLine($"Sending to PayBridge: {json}");

            var client = httpClientFactory.CreateClient("PayBridge");
            var response = await client.PostAsJsonAsync("/api/payments/initialize", payload);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"PayBridge Error: {errorContent}");
                ModelState.AddModelError("", $"Payment error: {errorContent}");
                return View(vm);
            }

            var result = await response.Content.ReadFromJsonAsync<PayBridgeInitResponse>();

            return Redirect(result!.AuthorizationUrl);
        }

        [HttpGet]
        [Route("Order/Completed")]
        public  async Task<IActionResult> CompletedAsync(int orderId)
        {
            var order = await orderRepository.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return NotFound();
            }


            var vm = new OrderCompletedViewModel
            {
                OrderId = orderId,
                OrderStatus = order.Status, // Show current status
                Message = order.Status switch
                {
                    OrderStatus.Pending => "Your payment is being processed. You'll receive an email confirmation shortly.",
                    OrderStatus.Success => "Payment successful! Your order has been confirmed.",
                    OrderStatus.Failed => "Payment failed. Please try again.",
                    _ => "Processing..."
                }
            };
            return View("Completed", vm);
        }
    }
}
