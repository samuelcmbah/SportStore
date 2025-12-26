
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

        public IQueryable<Order> Orders => context.Orders.Include(opt => opt.OrderItems).ThenInclude(opt => opt.Product);

        public Order GetOrder(int id)
        {
            return context.Orders.Find(id);
        }

        public void SaveOrder(Order order)
        {
            context.AttachRange(order.OrderItems.Select(opt => opt.Product));

            if (order.OrderID == 0)
            {
                context.Orders.Add(order);
            }

            context.SaveChanges();
        }

    }
}
