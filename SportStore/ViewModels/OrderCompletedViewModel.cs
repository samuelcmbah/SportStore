using SportStore.Models.Enums;

namespace SportStore.ViewModels
{
    public class OrderCompletedViewModel
    {
        public int OrderId { get; set; }
        public OrderStatus  OrderStatus { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
