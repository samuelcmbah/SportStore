namespace SportStore.Models.ViewModels
{
    public class CartViewModel
    {
        public List<CartItem?> CartItems { get; set; } = new();
        public int? TotalCartItems { get; set; }
    }
}
