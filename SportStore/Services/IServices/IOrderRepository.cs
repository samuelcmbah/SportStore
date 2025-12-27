using SportStore.Models;

namespace SportStore.Services.IServices
{
    public interface IOrderRepository
    {
        IQueryable<Order> Orders { get; }

        Order GetOrder(int id);

        void SaveOrder (Order order);   
    }
}
