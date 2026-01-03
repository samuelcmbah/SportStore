using SportStore.Models;

namespace SportStore.Services.IServices
{
    public interface ICategoryService
    {
        Task<Category> CreateAsync(Category category);
        Task<bool> DeleteAsync(int id);
        IQueryable<Category> GetAll();
        Task<Category?> GetByIdAsync(int id);
        Task<Category> UpdateAsync(Category category);

    }
}
