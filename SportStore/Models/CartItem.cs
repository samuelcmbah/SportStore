namespace SportStore.Models
{
    public class CartItem
    {
        public int CartItemID { get; set; }
        public Product? Product { get; set; } = new();
        public int Quantity { get; set; }
    }
}
