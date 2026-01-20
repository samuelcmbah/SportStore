using SportStore.Dtos;
using SportStore.Models;
using SportStore.Services.IServices;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace SportStore.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PaymentService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> InitializeCheckoutAsync(
            Order order,
            decimal amount,
            string userEmail,
            string redirectUrl)
        {
            // Build the payload for PayBridge
            var payload = new
            {
                ExternalUserId = userEmail,
                Amount = amount,
                Purpose = 0,  // ProductCheckout
                Provider = 0, // Paystack  
                AppName = "SportStore",
                ExternalReference = order.OrderID.ToString(),
                RedirectUrl = redirectUrl,
                NotificationUrl = "https://localhost:7001/api/notifications/paybridge"
            };

            var client = _httpClientFactory.CreateClient("PayBridge");
            var response = await client.PostAsJsonAsync("/api/payments/initialize", payload);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Payment initialization failed: {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<PayBridgeInitResponse>();

            if (result == null || string.IsNullOrEmpty(result.AuthorizationUrl))
                throw new Exception("Invalid response from payment provider.");

            return result.AuthorizationUrl;
        }
    }
}
