using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportStore.Models;
using SportStore.Services.IServices;

namespace SportStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Administrator")]
    public class OrdersController : Controller
    {
        private readonly IOrderRepository orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        // GET: Admin/Orders
        public IActionResult Index()
        {
            var orders = orderRepository.GetAllOrders()
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await orderRepository.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/Orders/MarkShipped/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkShipped(int id)
        {
            try
            {
                await orderRepository.MarkOrderAsShippedAsync(id);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Orders/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var order = await orderRepository.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await orderRepository.DeleteOrderAsync(id);
            return RedirectToAction(nameof(Index));
        }


    }
}
