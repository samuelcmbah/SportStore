using SportStore.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportStore.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; } 

        public List<CartItem> CartItems { get; set; } = new();

        public int? TotalCartItems { get; set; }

        [Column(TypeName = "decimal(8, 2)")]
        public decimal Total { get; set; }

        // Add foreign key and navigation property to ApplicationUser
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
