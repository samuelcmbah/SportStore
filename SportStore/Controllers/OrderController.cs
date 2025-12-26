using Microsoft.AspNetCore.Mvc;
using SportStore.Models;
using SportStore.Services.IServices;
using SportStore.Utils;

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

                order.OrderDate = DateTime.Now;
                order.OrderItems = cart.CartItems.Select(cartItem => new OrderItem
                {
                    Product = cartItem.Product,
                    Quantity = cartItem.Quantity
                    // You can set other properties of the order items here
                }).ToList();
                orderRepository.SaveOrder(order);
                cart.CartItems.Clear();
                cart.TotalCartItems = null;

                sessionCart.RemoveCart();
                sessionCart.SetCart(cart);

                return View("Completed", new { orderId = order.OrderID });
            }

            return View();
        }
    }
}
