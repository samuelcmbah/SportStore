using Microsoft.AspNetCore.Mvc.Rendering;
using SportStore.Models;
using System.ComponentModel.DataAnnotations;

namespace SportStore.ViewModels.ProductVM
{
    public class ProductCreateViewModel
    {
        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Description { get; set; } = "";

        // Selected category
        public int CategoryId { get; set; }

        // Dropdown data
        public IEnumerable<SelectListItem> Categories { get; set; }
            = Enumerable.Empty<SelectListItem>();

        [Required]
        public decimal Price { get; set; }

        public IFormFile? Photo { get; set; }
    }
}
