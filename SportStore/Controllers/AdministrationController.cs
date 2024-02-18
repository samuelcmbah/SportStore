using Microsoft.AspNetCore.Mvc;
using SportStore.Models;
using SportStore.Models.ViewModels;

namespace SportStore.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly IStoreRepository storeRepository;
        private readonly IOrderRepository orderRepository;

        public AdministrationController(IStoreRepository storeRepository, IOrderRepository orderRepository)
        {
            this.storeRepository = storeRepository;
            this.orderRepository = orderRepository;
        }

        public IActionResult ManageProducts()
        {
            var model = new ProductsListViewModel
            {
                Products = storeRepository.AllProducts
            };
            return View(model);
        }

        public IActionResult ManageOrders(int orderId)
        {
            if(orderId != 0)
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
    }
}
