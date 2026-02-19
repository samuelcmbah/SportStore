using SportStore.Models;

namespace SportStore.ViewModels.CartVM
{
    public class CartViewModel
    {
        public Cart Cart { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public int TotalItems { get; set; }
    }

}
