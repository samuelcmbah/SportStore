using SportStore.Models;

namespace SportStore.Services.IServices
{
    public interface ICartService
    {
        Task<Cart> GetOrCreateCartByUserIdAsync(string userId); 
        Task UpdateCartAsync(Cart cart);
    }
}
