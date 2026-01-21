using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportStore.Services.IServices;
using System.Security.Claims;

namespace SportStore.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersApiController : ControllerBase
    {
        private readonly IOrderRepository orderRepository;

        public OrdersApiController(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        [HttpGet("{orderId}/status")]
        public async Task<IActionResult> GetOrderStatus(int orderId)
        {
            var order = await orderRepository.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (order.UserId != userId && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }

            return Ok(new
            {
                status = order.Status.ToString(),
                orderId = order.OrderID
            });
        }
    }
}
