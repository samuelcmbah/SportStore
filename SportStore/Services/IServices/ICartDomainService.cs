using Serilog;
using SportStore.Models;

namespace SportStore.Services.IServices
{
    public interface ICartDomainService
    {
        void AddItem(Cart cart, Product product, int quantity = 1);
        void RemoveItem(Cart cart, long productId);
        void Merge(Cart targetCart, Cart sourceCart);
        int GetTotalItems(Cart cart);
        decimal GetTotalPrice(Cart cart);
    }
}
