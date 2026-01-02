using Microsoft.EntityFrameworkCore;
using SportStore.Models;
using SportStore.ViewModels.CartVM;

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
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Order> Orders => Set<Order>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //adds a Unique index on Cart.UserId
            modelBuilder.Entity<Cart>()
            .HasIndex(c => c.UserId)
            .IsUnique();
        }
    }
}

