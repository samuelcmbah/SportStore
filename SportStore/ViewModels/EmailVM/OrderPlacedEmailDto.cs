namespace SportStore.ViewModels.EmailVM
{
    public class OrderPlacedEmailDto
    {
        public string CustomerName { get; set; } = "";
        public string Email { get; set; } = "";
        public string OrderRef { get; set; } = "";
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public List<OrderPlacedItemDto> Items { get; set; } = new();
    }
}
