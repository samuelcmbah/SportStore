using System.ComponentModel.DataAnnotations;

namespace SportStore.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        public Product Product { get; set; } = new();

        public int OrderId { get; set; }

        public int Quantity { get; set; }
    }
}
