using Microsoft.AspNetCore.Mvc;
using SportStore.Models;
using SportStore.Services;
using SportStore.Services.IServices;
using SportStore.Utils;
using SportStore.ViewModels;
using System.Security.Claims;

namespace SportStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly ICartService cartService;
        private readonly SessionCart sessionCart;
        private readonly ICartDomainService cartDomainService;
        private readonly IOrderDomainService orderDomainService;
        private readonly IOrderRepository orderRepository;

        public OrderController(
            ICartService cartService,
            SessionCart sessionCart,
            ICartDomainService cartDomainService,
            IOrderDomainService orderDomainService,
            IOrderRepository orderRepository)
        {
            this.cartService = cartService;
            this.sessionCart = sessionCart;
            this.cartDomainService = cartDomainService;
            this.orderDomainService = orderDomainService;
            this.orderRepository = orderRepository;
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

            var order = orderDomainService.CreateOrderFromCart(cart, vm, userId);

            orderRepository.SaveOrder(order);

            // Clear cart
            cart.CartItems.Clear();
            if (User.Identity!.IsAuthenticated)
                await cartService.UpdateCartAsync(cart);
            else
                sessionCart.ClearCart();

            // Email will go here
            return RedirectToAction("Completed", new { orderId = order.OrderID });
        }

        [HttpGet]
        public  IActionResult Completed(int orderId)
        {
            //sending mail to user goes in here.
            var vm = new OrderCompletedViewModel
            {
                OrderId = orderId
            };
            return View(vm);
        }
    }
}
