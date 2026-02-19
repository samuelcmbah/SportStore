using SportStore.Models;

namespace SportStore.Services.IServices
{
    public interface IInventoryService
    {

        Task ReduceInventoryForOrderAsync(Order order);
        Task<List<string>> ValidateCartStockAsync(Cart cart);
        Task<bool> IsProductInStockAsync(long productId, int quantity);

    }
}
