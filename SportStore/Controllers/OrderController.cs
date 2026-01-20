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
        private readonly ICurrentUserService currentUserService;
        private readonly IPaymentService paymentService;
        private readonly ILogger<OrderController> logger;

        public OrderController(
            ICartService cartService,
            SessionCart sessionCart,
            ICartDomainService cartDomainService,
            IOrderDomainService orderDomainService,
            IOrderRepository orderRepository,
            IOrderNotificationService orderNotificationService,
            ICurrentUserService currentUserService,
            IPaymentService paymentService,
            ILogger<OrderController> logger)
        {
            this.cartService = cartService;
            this.sessionCart = sessionCart;
            this.cartDomainService = cartDomainService;
            this.orderDomainService = orderDomainService;
            this.orderRepository = orderRepository;
            this.orderNotificationService = orderNotificationService;
            this.currentUserService = currentUserService;
            this.paymentService = paymentService;
            this.logger = logger;
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
            var totalPrice = cartDomainService.GetTotalPrice(cart);
            if (!ModelState.IsValid)
            {
                vm.TotalPrice = totalPrice;
                return View(vm);
            }

            var userId = currentUserService.UserId;
            var userEmail = currentUserService.Email;

            var order = orderDomainService.CreateOrderFromCart(cart, vm, userId);
            await orderRepository.CreateOrderAsync(order);

            // Controller generates the redirect URL and passes to service
            var redirectUrl = Url.Action("Completed", "Order", new { orderId = order.OrderID }, Request.Scheme);
            if (string.IsNullOrWhiteSpace(redirectUrl))
            {
                // Log this — it should NEVER happen in normal flow
                logger.LogError(
                    "Failed to generate redirect URL for order {OrderId}",
                    order.OrderID
                );

                ModelState.AddModelError(
                    string.Empty,
                    "Unable to start payment at this time. Please try again later."
                );

                vm.TotalPrice = totalPrice;
                return View(vm);
            }
            string authorizationUrl;
            try
            {
                authorizationUrl = await paymentService.InitializeCheckoutAsync(order, totalPrice, userEmail, redirectUrl);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                vm.TotalPrice = totalPrice;
                return View(vm);
            }

            return Redirect(authorizationUrl);
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
                    OrderStatus.Pending => "Your payment is being processed.",
                    OrderStatus.Success => "Payment successful! Your order has been confirmed.",
                    OrderStatus.Failed => "Payment failed. Please try again.",
                    _ => "Processing..."
                }
            };
            return View("Completed", vm);
        }
    }
}
