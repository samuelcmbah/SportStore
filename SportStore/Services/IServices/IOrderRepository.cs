using SportStore.Models;

namespace SportStore.Services.IServices
{

    public interface IOrderRepository
    {
        IQueryable<Order> GetAllOrders();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<IEnumerable<Order>> GetOrdersByUserAsync(string userId);

        Task CreateOrderAsync(Order order);
        Task MarkOrderAsShippedAsync(int orderId);

        Task<bool> DeleteOrderAsync(int orderId);
        Task UpdateOrderAsync(Order order);
        Task<Order> GetOrderByReferenceAsync(string orderRef);
    }

}
