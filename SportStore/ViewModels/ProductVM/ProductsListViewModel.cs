

using SportStore.Models;

namespace SportStore.ViewModels.ProductVM
{
    public class ProductsListViewModel
    {
        public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();
        public PagingInfoViewModel PagingInfo { get; set; } = new();
        public int? CurrentCategoryId { get; set; }
    }
}
