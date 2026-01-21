using SportStore.Models;

namespace SportStore.Services.IServices
{
    public interface IPaymentService
    {
        Task<string> InitializeCheckoutAsync(string orderRef, decimal amount, string userEmail,
            string redirectUrl);
    }
}
