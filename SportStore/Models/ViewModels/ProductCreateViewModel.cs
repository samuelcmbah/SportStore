using System.ComponentModel.DataAnnotations;

namespace SportStore.Models.ViewModels
{
    public class ProductCreateViewModel
    {
        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Description { get; set; } = "";

        [Required]
        public string Cartegory { get; set; } = "";

        [Required]
        public decimal Price { get; set; }

        public IFormFile? Photo { get; set; }
    }
}
