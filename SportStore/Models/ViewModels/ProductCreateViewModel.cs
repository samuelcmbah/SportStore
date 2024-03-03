namespace SportStore.Models.ViewModels
{
    public class ProductCreateViewModel
    {
        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public string Cartegory { get; set; } = "";

        public decimal Price { get; set; }
    }
}
