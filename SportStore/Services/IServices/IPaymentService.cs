using SportStore.Models;

namespace SportStore.Services.IServices
{
    public interface IPaymentService
    {
        Task<string> InitializeCheckoutAsync(Order order, decimal amount, string userEmail,
            string redirectUrl);
    }
}
