using Microsoft.EntityFrameworkCore;
using SportStore.Data;
using SportStore.Models;
using SportStore.Services.IServices;
using SportStore.ViewModels.EmailVM;

namespace SportStore.Services
{
    public class OrderNotificationService : IOrderNotificationService
    {
        private readonly IEmailService emailService;
        private readonly StoreDbContext context;
        private readonly ILogger<OrderNotificationService> logger;

        public OrderNotificationService(
            IEmailService emailService, 
            StoreDbContext context,
            ILogger<OrderNotificationService> logger)
        {
            this.emailService = emailService;
            this.context = context;
            this.logger = logger;
        }

        public async Task SendOrderPlacedEmailAsync(Order order)
        {
            var savedOrder = await context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderID == order.OrderID);

            if (savedOrder == null)
            {
                logger.LogWarning(
                    "OrderPlaced email skipped: Order {OrderId} not found",
                    order.OrderID
                );
                return;
            }

            var emailDto = MapToEmailDto(savedOrder);

            var html = BuildOrderPlacedHtml(emailDto);

            await emailService.SendEmailAsync(
                savedOrder.Email,
                $"{savedOrder.OrderReference} Confirmation",
                html
            );
        }


        private OrderPlacedEmailDto MapToEmailDto(Order order)
        {
            return new OrderPlacedEmailDto
            {
                OrderRef = order.OrderReference,
                OrderDate = order.OrderDate,
                CustomerName = order.Name!,
                Email = order.Email!,
                TotalAmount = order.OrderItems.Sum(i => i.Quantity * i.Product.Price),
                Items = order.OrderItems.Select(i => new OrderPlacedItemDto
                {
                    ProductName = i.Product.Name,
                    UnitPrice = i.Product.Price,
                    Quantity = i.Quantity
                }).ToList()
            };
        }

        private string BuildOrderPlacedHtml(OrderPlacedEmailDto order)
        {
            var rows = string.Join("", order.Items.Select(i => $@"
                <tr>
                    <td style='padding:8px;border:1px solid #ddd;'>{i.ProductName}</td>
                    <td style='padding:8px;border:1px solid #ddd;'>{i.UnitPrice:C}</td>
                    <td style='padding:8px;border:1px solid #ddd;'>{i.Quantity}</td>
                    <td style='padding:8px;border:1px solid #ddd;'>{i.Subtotal:C}</td>
                </tr>
            "));

            return $@"
                <h2>Thank you for your order, {order.CustomerName}!</h2>
                <p>Your order <strong>{order.OrderRef}</strong> has been successfully placed.</p>

                <table style='border-collapse:collapse;width:100%;margin-top:20px;'>
                    <thead>
                        <tr>
                            <th style='padding:8px;border:1px solid #ddd;'>Product</th>
                            <th style='padding:8px;border:1px solid #ddd;'>Price</th>
                            <th style='padding:8px;border:1px solid #ddd;'>Qty</th>
                            <th style='padding:8px;border:1px solid #ddd;'>Subtotal</th>
                        </tr>
                    </thead>
                    <tbody>
                        {rows}
                    </tbody>
                </table>

                <h3 style='margin-top:20px;'>Total: {order.TotalAmount:C}</h3>

                <p>We are processing your order and will notify you once it ships.</p>

                <p>– SportStore Team</p>
            ";
        }

    }
}
