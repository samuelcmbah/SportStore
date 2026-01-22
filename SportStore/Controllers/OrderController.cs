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
    [Authorize]
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
        private readonly IInventoryService inventoryService;
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
            IInventoryService inventoryService,
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
            this.inventoryService = inventoryService;
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
                TotalPrice = cartDomainService.GetTotalPrice(cart),
                CartItems = cart.CartItems
            };

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel vm)
        {
            var cart = await GetCartAsync();
            if (cart == null || !cart.CartItems.Any())
            {
                ModelState.AddModelError("", "Your cart is empty.");
                return RedirectToAction("ViewCart", "Cart"); 
            }

            // Validate stock availability before payment
            var stockErrors = await inventoryService.ValidateCartStockAsync(cart);
            if (stockErrors.Any())
            {
                foreach (var error in stockErrors)
                {
                    ModelState.AddModelError("", error);
                }
                vm.TotalPrice = cartDomainService.GetTotalPrice(cart);
                return View(vm);
            }

            if (!ModelState.IsValid)
            {
                RePopulateViewModel(vm, cart);
                return View(vm);
            }

            try
            {
                var userId = currentUserService.UserId;
                var userEmail = currentUserService.Email;
                var totalPrice = cartDomainService.GetTotalPrice(cart);

                // Create Order Object (Wait to save to DB until payment is ready or use a transaction)
                var order = orderDomainService.CreateOrderFromCart(cart, vm, userId);

                // Generate Callback URL
                var redirectUrl = Url.Action("Completed", "Order", new { orderRef = order.OrderReference }, Request.Scheme);

                if (redirectUrl is null)
                {
                    logger.LogCritical("Routing Error: Could not generate Redirect URL for Order {OrderId}. Check 'Order/Completed' route configuration.", order.OrderID);

                    ModelState.AddModelError(string.Empty, "A system error occurred while preparing your payment. Please contact support.");

                    RePopulateViewModel(vm, cart);
                    return View(vm);
                }
                var authorizationUrl = await paymentService.InitializeCheckoutAsync(order.OrderReference, totalPrice, userEmail, redirectUrl);

                // Persist Order only after successful payment initialization
                await orderRepository.CreateOrderAsync(order);

                return Redirect(authorizationUrl);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Checkout failure for user {Email}", currentUserService.Email);

                ModelState.AddModelError(string.Empty, "Unable to process payment at the moment. Please try again.");

                RePopulateViewModel(vm, cart);
                return View(vm);
            }
        }

        /// <summary>
        /// Helper to ensure the view doesn't crash when returning due to validation errors
        /// </summary>
        private void RePopulateViewModel(CheckoutViewModel vm, Cart cart)
        {
            vm.CartItems = cart.CartItems; // Critical: Prevents NullReference in @foreach
            vm.TotalPrice = cartDomainService.GetTotalPrice(cart);
        }

        [HttpGet]
        [Route("Order/Completed")]
        public async Task<IActionResult> CompletedAsync(string orderRef)
        {
            //find using fist or default with orderref
            var order = await orderRepository.GetOrderByReferenceAsync(orderRef);

            if (order == null)
            {
                return NotFound();
            }


            var vm = new OrderCompletedViewModel
            {
                OrderRef = order.OrderReference,
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
