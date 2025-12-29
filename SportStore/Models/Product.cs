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

        public string Category { get; set; } = String.Empty;

        public string? PhotoPath { get; set; }
    }
}
