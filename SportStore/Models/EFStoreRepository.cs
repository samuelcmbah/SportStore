
using Microsoft.EntityFrameworkCore;

namespace SportStore.Models
{
    public class EFStoreRepository : IStoreRepository
    {
        private readonly StoreDbContext context;

        public EFStoreRepository(StoreDbContext context)
        {
            this.context = context;
        }

        public IQueryable<Product> AllProducts => context.Products; // returning the DbSet of products

        public Product? CreateProduct(Product product)
        {
            context.Products.Add(product);
            context.SaveChanges();
            return product;
        }

        public Product? DeleteProduct(Product product)
        {
            context.Entry(product).State = EntityState.Deleted;
            context.SaveChanges();
            return product;
        }

        public Product? GetProductById(long? id)
        {
            return context.Products.Find(id);
        }

        public Product? UpdateProduct(Product? editedProduct)
        {
            var product = context.Products.Attach(editedProduct);
            product.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return (editedProduct);

        }
    }
}
