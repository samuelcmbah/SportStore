
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using SportStore.Dtos;
using SportStore.Models;
using SportStore.Models.Enums;
using SportStore.Services;
using SportStore.Services.IServices;
using SportStore.Utils;

namespace SportStore.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [AllowAnonymous] //  webhooks don't have user authentication
    public class PaymentNotificationAPIController : ControllerBase
    {
        private readonly IOrderRepository orderRepository;
        private readonly IOrderNotificationService notificationService;
        private readonly ICartService cartService;
        private readonly SessionCart sessionCart;

        public PaymentNotificationAPIController(IOrderRepository orderRepository, IOrderNotificationService notificationService,
            ICartService cartService, SessionCart sessionCart)
        {
            this.orderRepository = orderRepository;
            this.notificationService = notificationService;
            this.cartService = cartService;
            this.sessionCart = sessionCart;
        }

        [HttpPost("paybridge")]
        public async Task<IActionResult> Handle([FromBody] PayBridgeNotification payload)
        {
            if (string.IsNullOrEmpty(payload.PaymentReference) ||
                    string.IsNullOrEmpty(payload.ExternalReference))
            {
                return BadRequest("Invalid payload");
            }


            var order = await orderRepository.GetOrderByIdAsync(int.Parse(payload.ExternalReference));

            if (order == null)
            {
                return NotFound("Order not found");
            }

            if (order.Status != OrderStatus.Pending)
            {
                return Ok("Already processed");
            }

            // Update order status based on payment
            if (payload.Status == "Success")
            {
                order.Status = OrderStatus.Success;
                order.PaymentReference = payload.PaymentReference;
                order.PaidAt = DateTime.UtcNow;

                await orderRepository.UpdateOrderAsync(order);

                // Send confirmation email AFTER payment verified
                await notificationService.SendOrderPlacedEmailAsync(order);


                if (!string.IsNullOrEmpty(order.UserId))
                {
                    var cart = await cartService.GetOrCreateCartByUserIdAsync(order.UserId);
                    cart.CartItems.Clear();
                    await cartService.UpdateCartAsync(cart);
                }
            }
            else
            {
                order.Status = OrderStatus.Failed;
                await orderRepository.UpdateOrderAsync(order);
            }

            return Ok();
        }

       
    }
}
