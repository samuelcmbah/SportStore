using Microsoft.EntityFrameworkCore;

namespace SportStore.Models
{
    public static class SeedData 
    {
        public static void EnsurePopulated(IApplicationBuilder app) 
        {
            //Gets the required DbContext services
            StoreDbContext context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<StoreDbContext>();

            
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
            if (!context.Products.Any())
            {
                context.Products.AddRange(
                new Product
                {
                    Name = "Kayak",
                    Description = "A boat for one person",
                    Category = "Watersports",
                    Price = 275,
                    PhotoPath = "kayak.jfif"
                },
                new Product
                {
                    Name = "Lifejacket",
                    Description = "Protective and fashionable",
                    Category = "Watersports",
                    Price = 48.95m,
                    PhotoPath = "lifejacket.jfif"
                },
                new Product
                {
                    Name = "Soccer Ball",
                    Description = "FIFA-approved size and weight",
                    Category = "Soccer",
                    Price = 19.50m,
                    PhotoPath = "soccer ball.jfif"
                },
                new Product
                {
                    Name = "Corner Flags",
                    Description = "Give your playing field a professional touch",
                    Category = "Soccer",
                    Price = 34.95m,
                    PhotoPath = "corner flags.jfif"
                },
                new Product
                {
                    Name = "Stadium",
                    Description = "Flat-packed 35,000-seat stadium",
                    Category = "Soccer",
                    Price = 79500,
                    PhotoPath = "stadium.jfif"
                },
                new Product
                {
                    Name = "Thinking Cap",
                    Description = "Improve brain efficiency by 75%",
                    Category = "Chess",
                    Price = 16,
                    PhotoPath = "thinking cap.jfif"
                },
                new Product
                {
                    Name = "Unsteady Chair",
                    Description = "Secretly give your opponent a disadvantage",
                    Category = "Chess",
                    Price = 29.95m,
                    PhotoPath = "unsteady chair.jfif"
                },
                new Product
                {
                    Name = "Human Chess Board",
                    Description = "A fun game for the family",
                    Category = "Chess",
                    Price = 75,
                    PhotoPath = "chess board.jfif"
                },
                new Product
                {
                    Name = "Roller Blades",
                    Description = "flow with the wind",
                    Category = "Action",
                    Price = 1200,
                    PhotoPath = "roller blades.jfif"
                }
                );
                context.SaveChanges();
            }
        }
    }
}
