namespace SportStore.Models
{
    public interface IOrderRepository
    {
        IQueryable<Order> Orders { get; }

        Order GetOrder(int id);

        void SaveOrder (Order order);   
    }
}
