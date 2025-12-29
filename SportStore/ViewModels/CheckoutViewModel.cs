using System.ComponentModel.DataAnnotations;

namespace SportStore.ViewModels
{
    public class CheckoutViewModel
    {
        // Shipping info
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Line1 { get; set; } = string.Empty;

        public string? Line2 { get; set; }
        public string? Line3 { get; set; }

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string State { get; set; } = string.Empty;

        public string? Zip { get; set; }

        [Required]
        public string Country { get; set; } = string.Empty;

        public bool GiftWrap { get; set; }

        // Display-only
        public decimal TotalPrice { get; set; }
    }

}
