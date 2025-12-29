namespace SportStore.ViewModels.EmailVM
{
    public class OrderPlacedItemDto
    {
        public string ProductName { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => UnitPrice * Quantity;
    }
}
