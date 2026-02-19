using SportStore.Models;
using SportStore.ViewModels;
using SportStore.ViewModels.ProductVM;

namespace SportStore.Services.IServices
{
    public interface IProductService
    {
        IQueryable<Product> GetAll();

        Task<Product?> GetByIdAsync(long id);
        Task<Product> CreateAsync(ProductCreateViewModel product);
        Task<Product> UpdateAsync(ProductEditViewModel product);
        Task<bool> DeleteAsync(long id);

        IQueryable<Product> SearchFilter(ProductSearchFilterQuery query);

    }
}
