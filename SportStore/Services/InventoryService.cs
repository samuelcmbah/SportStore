using Microsoft.EntityFrameworkCore;
using SportStore.Data;
using SportStore.Models;
using SportStore.Services.IServices;

namespace SportStore.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly StoreDbContext context;
        private readonly ILogger<InventoryService> logger;

        public InventoryService(StoreDbContext context, ILogger<InventoryService> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public async Task<bool> IsProductInStockAsync(long productId, int quantity)
        {
            var product = await context.Products
                 .AsNoTracking()
                 .FirstOrDefaultAsync(p => p.ProductID == productId);

            return product != null && product.StockQuantity >= quantity;
        }

        public async Task ReduceInventoryForOrderAsync(Order order)
        {
            foreach (var orderItem in order.OrderItems)
            {
                var product = await context.Products.FindAsync(orderItem.ProductId);

                if (product == null)
                {
                    logger.LogError("Product {ProductId} not found when reducing inventory for order {OrderId}",
                        orderItem.ProductId, order.OrderID);
                    throw new InvalidOperationException($"Product {orderItem.ProductId} not found");
                }

                if (product.StockQuantity < orderItem.Quantity)
                {
                    logger.LogWarning(
                        "Insufficient stock for product {ProductId}. Available: {Available}, Requested: {Requested}",
                        product.ProductID, product.StockQuantity, orderItem.Quantity);

                    throw new InvalidOperationException(
                        $"Insufficient stock for {product.Name}. Available: {product.StockQuantity}, Requested: {orderItem.Quantity}"
                    );
                }

                product.StockQuantity -= orderItem.Quantity;

                logger.LogInformation(
                    "Reduced stock for product {ProductId} by {Quantity}. New stock: {NewStock}",
                    product.ProductID, orderItem.Quantity, product.StockQuantity);
            }

            await context.SaveChangesAsync();
        }

        public async Task<List<string>> ValidateCartStockAsync(Cart cart)
        {
            var errors = new List<string>();

            foreach (var item in cart.CartItems)
            {
                var product = await context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.ProductID == item.ProductId);

                if (product == null)
                {
                    errors.Add($"{item.Product.Name}: Product not found");
                    logger.LogWarning("Product {ProductId} not found during cart validation", item.ProductId);
                    continue;
                }

                if (product.StockQuantity < item.Quantity)
                {
                    errors.Add($"{item.Product.Name}: Only {product.StockQuantity} available, you requested {item.Quantity}");
                    logger.LogInformation(
                        "Insufficient stock for product {ProductId} in cart. Available: {Available}, Requested: {Requested}",
                        product.ProductID, product.StockQuantity, item.Quantity);
                }
            }

            return errors;
        }
    }
}
