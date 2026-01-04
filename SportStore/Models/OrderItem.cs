using System.ComponentModel.DataAnnotations;

namespace SportStore.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        public int Quantity { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public long ProductId { get; set; }
        public Product Product { get; set; } = new();


    }
}
