using SportStore.Models;

namespace SportStore.Services.IServices
{
    public interface IProductService
    {
        IQueryable<Product> GetAll();

        Task<Product?> GetByIdAsync(long id);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task<bool> DeleteAsync(long id);
    }
}
