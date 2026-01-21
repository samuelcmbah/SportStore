using SportStore.ViewModels.CartVM;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportStore.Models
{
    public class Product
    {
        public long ProductID { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;


        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        // FK
        public int CategoryId { get; set; }

        // Navigation property
        public Category Category { get; set; } = null!;

        public string? PhotoPath { get; set; }

        // NEW: Stock quantity
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be 0 or greater")]
        public int StockQuantity { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Helper property to check if product is in stock
        [NotMapped]
        public bool IsInStock => StockQuantity > 0;

        // Helper to check if a specific quantity is available
        public bool HasStock(int requestedQuantity) => StockQuantity >= requestedQuantity;

    }
}
