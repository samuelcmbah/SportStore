using SportStore.Models;

namespace SportStore.ViewModels.ProductVM
{
    public class ProductDetailsViewModel
    {
        // Product data (read-only for the view)
        public Product Product { get; set; } = null!;

        // Cart-related UI state
        public bool IsInCart { get; set; }
        public int QuantityInCart { get; set; }

        // UI helpers
        public int MaxQuantity { get; set; }
        public string PhotoPath =>
            "/images/" + (Product.PhotoPath ?? "noimage.png");
    }
}
