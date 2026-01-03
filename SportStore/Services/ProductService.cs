using Microsoft.EntityFrameworkCore;
using SportStore.Data;
using SportStore.Models;
using SportStore.Services.IServices;

namespace SportStore.Services
{
    public class ProductService : IProductService
    {
        private readonly StoreDbContext context;

        public ProductService(StoreDbContext context)
        {
            this.context = context;
        }

        public IQueryable<Product> GetAll()
        {
            return context.Products
                .Include(p => p.Category)
                .AsQueryable();
        }

        public async Task<Product?> GetByIdAsync(long id)
        {
            return await context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductID == id);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            context.Products.Add(product);
            await context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            context.Products.Update(product);
            await context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var product = await context.Products.FindAsync(id);

            if (product == null)
                return false;

            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
