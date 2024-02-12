using Microsoft.AspNetCore.Mvc;
using SportStore.Models;

namespace SportStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository orderRepository;
        private readonly SessionCart sessionCart;

        public OrderController(IOrderRepository orderRepository, SessionCart sessionCart)
        {
            this.orderRepository = orderRepository;
            this.sessionCart = sessionCart;
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            var cart = sessionCart.GetCart();

            if (!cart.CartItems.Any())
            {
                ModelState.AddModelError(string.Empty, "your cart is empty");
            }
            if (ModelState.IsValid)
            {
                order.CartItems = cart.CartItems.ToArray();
                orderRepository.SaveOrder(order);
                cart.CartItems.Clear();

                sessionCart.RemoveCart();
                sessionCart.SetCart(cart);

                return View("Completed", new { orderId = order.OrderID });
            }

            return View();
        }
    }
}
