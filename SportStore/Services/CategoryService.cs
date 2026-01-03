using Microsoft.EntityFrameworkCore;
using SportStore.Data;
using SportStore.Models;
using SportStore.Services.IServices;

namespace SportStore.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly StoreDbContext context;

        public CategoryService(StoreDbContext context)
        {
            this.context = context;
        }
        public async Task<Category> CreateAsync(Category category)
        {
            context.Categories.Add(category);
            await context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return false;
            }
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return true;
        }

        public IQueryable<Category> GetAll()
        {
            return context.Categories;
        }
        // use FindAsync when: querying by primary key,don’t need Include
        //want max efficiency, may already have the entity tracked
        public async Task<Category?> GetByIdAsync(int id)
        {
            return await context.Categories.FindAsync(id);
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            context.Categories.Update(category);
            await context.SaveChangesAsync();
            return category;
        }
    }
}
