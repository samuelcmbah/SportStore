using Microsoft.EntityFrameworkCore;

namespace SportStore.Models
{
    public class StoreDbContext : DbContext
    {

        public StoreDbContext(DbContextOptions<StoreDbContext> options)
        : base(options)
        {
           
        }

        public DbSet<Product> Products => Set<Product>();

        public DbSet<Order> Orders => Set<Order>();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            var dbPath = System.IO.Path.Join(path, "SportStore.db");
            options.UseSqlite($"Data Source={dbPath}");
        }
    }
}

