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

        // an external identifier not fk, as we are using two db contexts
        public string UserId { get; set; } = null!;
        //public ApplicationUser? User { get; set; }
    }
}
