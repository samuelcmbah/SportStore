
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
        private readonly IInventoryService inventoryService;
        private readonly ILogger<PaymentNotificationAPIController> logger;

        public PaymentNotificationAPIController(
            IOrderRepository orderRepository,
            IOrderNotificationService notificationService,
            ICartService cartService,
            IInventoryService inventoryService,
            ILogger<PaymentNotificationAPIController> logger)
        {
            this.orderRepository = orderRepository;
            this.notificationService = notificationService;
            this.cartService = cartService;
            this.inventoryService = inventoryService;
            this.logger = logger;
        }

        [HttpPost("paybridge")]
        public async Task<IActionResult> Handle([FromBody] PayBridgeNotification payload)
        {
            if (string.IsNullOrEmpty(payload.PaymentReference) ||
                 string.IsNullOrEmpty(payload.ExternalReference))
            {
                logger.LogWarning("Invalid payload received: {Payload}", payload);
                return BadRequest("Invalid payload");
            }

            var order = await orderRepository.GetOrderByReferenceAsync(payload.ExternalReference);

            if (order == null)
            {
                logger.LogWarning("Order not found for reference: {Reference}", payload.ExternalReference);
                return NotFound("Order not found");
            }

            if (order.Status != OrderStatus.Pending)
            {
                logger.LogInformation("Order {OrderRef} already processed with status {Status}",
                    order.OrderReference, order.Status);
                return Ok("Already processed");
            }

            // Update order status based on payment
            if (payload.Status == "Success")
            {
                try
                {
                    await inventoryService.ReduceInventoryForOrderAsync(order);

                    order.Status = OrderStatus.Success;
                    order.PaymentReference = payload.PaymentReference;
                    order.PaidAt = DateTime.UtcNow;

                    await orderRepository.UpdateOrderAsync(order);

                    // Send confirmation email AFTER payment verified
                    await notificationService.SendOrderPlacedEmailAsync(order);

                    // Clear cart
                    if (!string.IsNullOrEmpty(order.UserId))
                    {
                        var cart = await cartService.GetOrCreateCartByUserIdAsync(order.UserId);
                        cart.CartItems.Clear();
                        await cartService.UpdateCartAsync(cart);
                    }

                    logger.LogInformation("Order {OrderRef} completed successfully", order.OrderReference);
                }
                catch (InvalidOperationException ex)
                {
                    logger.LogError(ex, "Failed to reduce inventory for order {OrderRef}", order.OrderReference);

                    order.Status = OrderStatus.Failed;
                    await orderRepository.UpdateOrderAsync(order);
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                order.Status = OrderStatus.Failed;
                await orderRepository.UpdateOrderAsync(order);

                logger.LogInformation("Payment failed for order {OrderRef}", order.OrderReference);
            }

            return Ok();
        }

    }
}
        
