using SportStore.Models.Enums;

namespace SportStore.ViewModels
{
    public class OrderCompletedViewModel
    {
        public string OrderRef { get; set; } = string.Empty;
        public OrderStatus  OrderStatus { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
