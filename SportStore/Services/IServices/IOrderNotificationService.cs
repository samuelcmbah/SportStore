using SportStore.Models;

namespace SportStore.Services.IServices
{
    public interface IOrderNotificationService
    {
        Task SendOrderPlacedEmailAsync(Order order);
    }
}
