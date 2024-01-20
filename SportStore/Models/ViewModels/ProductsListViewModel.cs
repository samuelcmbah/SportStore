namespace SportStore.Models.ViewModels
{
    public class ProductsListViewModel
    {
        public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();

        public PagingInfoViewModel PagingInfo { get; set; } = new();

        public string? CurrentCategory { get; set; }
    }
}
