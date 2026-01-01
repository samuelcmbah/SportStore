using Microsoft.AspNetCore.Mvc;
using SportStore.Models;
using SportStore.Services.IServices;

namespace SportStore.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderRepository orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        public IActionResult ManageOrders(int orderId)
        {
            if (orderId != 0)
            {
                Order order = orderRepository.GetOrder(orderId);
                if (order == null)
                {
                    //take the user to a NotFound page
                }
                order.Shipped = true;
                orderRepository.SaveOrder(order);
            }

            var model = orderRepository.Orders.Where(o => !o.Shipped);
            return View(model);
        }

        public IActionResult ManageShippedOrders()
        {
            var model = orderRepository.Orders.Where(o => o.Shipped);
            return View(model);
        }
    }
}
