
using Microsoft.EntityFrameworkCore;
using SportStore.Data;
using SportStore.Models;
using SportStore.Services.IServices;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SportStore.Services
{
    public class EFOrderRepository : IOrderRepository
    {
        private readonly StoreDbContext context;

        public EFOrderRepository(StoreDbContext context)
        {
            this.context = context;
        }

        public IQueryable<Order> GetAllOrders()
        {
            return context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsNoTracking();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderID == id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserAsync(string userId)
        {
            return await context.Orders
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task CreateOrderAsync(Order order)
        {
            // Attach products to prevent EF from re-inserting them
            context.AttachRange(order.OrderItems.Select(opt => opt.Product));

            if (order.OrderID == 0)
            {
                context.Orders.Add(order);
            }

            await context.SaveChangesAsync();
        }

        public async Task MarkOrderAsShippedAsync(int orderId)
        {
            var order = await context.Orders.FindAsync(orderId);

            if (order == null)
            {
                throw new InvalidOperationException("Order not found.");
            }

            order.Shipped = true;
            order.ShippedDate = DateTime.UtcNow;

            await context.SaveChangesAsync();

            //send mail to usser
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await context.Orders.FindAsync(orderId);

            if (order == null)
            {
                return false;
            }

            context.Orders.Remove(order);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateOrderAsync(Order order)
        {
            context.Update(order);
            await context.SaveChangesAsync();
        }

        public async Task<Order> GetOrderByReferenceAsync(string orderRef)
        {
            var order = await context.Orders.FirstOrDefaultAsync(o => o.OrderReference == orderRef);
            if (order == null)
            {
                throw new InvalidOperationException("Order not found.");
            }
            return order;
        }
    }
}
