using SportStore.Models;
using SportStore.ViewModels;

namespace SportStore.Services.IServices
{
    public interface IOrderDomainService
    {
        Order CreateOrderFromCart(Cart cart, CheckoutViewModel vm, string? userId);

    }
}
