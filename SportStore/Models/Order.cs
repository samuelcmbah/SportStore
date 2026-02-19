using Microsoft.AspNetCore.Mvc.ModelBinding;
using SportStore.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace SportStore.Models
{
    public class Order
    {
        public int OrderID { get; set; }

        public string OrderReference { get; set; } = string.Empty;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public OrderStatus Status { get; set; }  
        public string PaymentReference { get; set; } = string.Empty;

        public string UserId { get; set; } = null!;

        public DateTime OrderDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public bool Shipped { get; set; }

        [Required(ErrorMessage = "Please enter a name")]
        public string? Name { get; set; }


        [Required(ErrorMessage = "Please enter a valid email")]
        [EmailAddress]
        public string Email { get; set; }  = string.Empty;


        [Required(ErrorMessage = "Please enter the first address line")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a city name")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a state name")]
        public string State { get; set; } = string.Empty;
        public string? Zip { get; set; }

        [Required(ErrorMessage = "Please enter a country name")]
        public string Country { get; set; } = string.Empty;

        public bool GiftWrap { get; set; }
        public DateTime PaidAt { get; internal set; }
    }
}
