using System.ComponentModel.DataAnnotations.Schema;

namespace SportStore.Models.ViewModels
{
    public class CartViewModel
    {
        public List<CartItem?> CartItems { get; set; } = new();

        public int? TotalCartItems { get; set; }

        [Column(TypeName = "decimal(8, 2)")]
        public decimal Total { get; set; }
    }
}
