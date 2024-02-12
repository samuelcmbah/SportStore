
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SportStore.Models
{
    public class EFOrderRepository : IOrderRepository
    {
        private readonly StoreDbContext context;

        public EFOrderRepository(StoreDbContext context)
        {
            this.context = context;
        }

        public IQueryable<Order> Orders => context.Orders.Include(opt => opt.CartItems).ThenInclude(opt => opt.Product);

        public void SaveOrder(Order order)
        {
            // For the Product objects associated with an Order,
            // this means that Entity Framework Core tries to write objects that have already been stored, which causes an error.
            // This ensures that Entity Framework Core won’t try to write the de-serialized Product objects that are associated with the Order object.
            context.AddRange(order.CartItems.Select(opt => opt.Product));
            if(order.OrderID == 0)
            {
                context.Orders.Add(order);
            }
            context.SaveChanges();
        }
    }
}
