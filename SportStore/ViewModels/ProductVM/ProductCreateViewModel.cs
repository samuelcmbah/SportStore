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

        [Required]
        public Category Cartegory { get; set; }

        [Required]
        public decimal Price { get; set; }

        public IFormFile? Photo { get; set; }
    }
}
