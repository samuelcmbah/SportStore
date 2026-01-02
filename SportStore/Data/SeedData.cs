using Microsoft.EntityFrameworkCore;
using SportStore.Models;

namespace SportStore.Data
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

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Watersports" },
                    new Category { Name = "Soccer" },
                    new Category { Name = "Chess" },
                    new Category { Name = "Action" }
                );

                context.SaveChanges();
            }
            // Converts the list of categories into a dictionary
            // where the key is the category's Name and the value is the full Category object.
            var categories = context.Categories.ToDictionary(c => c.Name);

            if (!context.Products.Any())
            {
                context.Products.AddRange(
                    new Product
                    {
                        Name = "Kayak",
                        Description = "A boat for one person",
                        Price = 275,
                        PhotoPath = "kayak.jfif",
                        CategoryId = categories["Watersports"].CategoryId
                    },
                    new Product
                    {
                        Name = "Lifejacket",
                        Description = "Protective and fashionable",
                        Price = 48.95m,
                        PhotoPath = "lifejacket.jfif",
                        CategoryId = categories["Watersports"].CategoryId
                    },
                    new Product
                    {
                        Name = "Soccer Ball",
                        Description = "FIFA-approved size and weight",
                        Price = 19.50m,
                        PhotoPath = "soccer ball.jfif",
                        CategoryId = categories["Soccer"].CategoryId
                    },
                    new Product
                    {
                        Name = "Corner Flags",
                        Description = "Give your playing field a professional touch",
                        Price = 34.95m,
                        PhotoPath = "corner flags.jfif",
                        CategoryId = categories["Soccer"].CategoryId
                    },
                    new Product
                    {
                        Name = "Stadium",
                        Description = "Flat-packed 35,000-seat stadium",
                        Price = 79500,
                        PhotoPath = "stadium.jfif",
                        CategoryId = categories["Soccer"].CategoryId
                    },
                    new Product
                    {
                        Name = "Thinking Cap",
                        Description = "Improve brain efficiency by 75%",
                        Price = 16,
                        PhotoPath = "thinking cap.jfif",
                        CategoryId = categories["Chess"].CategoryId
                    },
                    new Product
                    {
                        Name = "Unsteady Chair",
                        Description = "Secretly give your opponent a disadvantage",
                        Price = 29.95m,
                        PhotoPath = "unsteady chair.jfif",
                        CategoryId = categories["Chess"].CategoryId
                    },
                    new Product
                    {
                        Name = "Human Chess Board",
                        Description = "A fun game for the family",
                        Price = 75,
                        PhotoPath = "chess board.jfif",
                        CategoryId = categories["Chess"].CategoryId
                    },
                    new Product
                    {
                        Name = "Roller Blades",
                        Description = "Flow with the wind",
                        Price = 1200,
                        PhotoPath = "roller blades.jfif",
                        CategoryId = categories["Action"].CategoryId
                    }
                );

                context.SaveChanges();
            }

        }
    }
}
