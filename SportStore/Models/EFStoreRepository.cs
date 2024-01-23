
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

        public Product? GetProductById(long id)
        {
            return context.Products.Find(id);
        }
    }
}
