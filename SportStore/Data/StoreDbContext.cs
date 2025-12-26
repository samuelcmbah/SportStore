using Microsoft.EntityFrameworkCore;
using SportStore.Models;

namespace SportStore.Data
{
    public class StoreDbContext : DbContext
    {
        private readonly IConfiguration configuration;

        public StoreDbContext(DbContextOptions<StoreDbContext> options, IConfiguration configuration)
        : base(options)
        {
            this.configuration = configuration;
        }

        public DbSet<Product> Products => Set<Product>();

        public DbSet<Order> Orders => Set<Order>();

        
    }
}

