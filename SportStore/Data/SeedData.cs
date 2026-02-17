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
                        StockQuantity = 1000,
                        PhotoPath = "https://res.cloudinary.com/dpj4iudu1/image/upload/v1771280606/sportstore/products/pettuskulodmnkq6uzg1.jpg",
                        CategoryId = categories["Watersports"].CategoryId
                    },
                    new Product
                    {
                        Name = "Lifejacket",
                        Description = "Protective and fashionable",
                        Price = 48.95m,
                        StockQuantity = 1000,
                        PhotoPath = "https://res.cloudinary.com/dpj4iudu1/image/upload/v1771280535/sportstore/products/krt3tb1fit7cuquthyoz.jpg",
                        CategoryId = categories["Watersports"].CategoryId
                    },
                    new Product
                    {
                        Name = "Soccer Ball",
                        Description = "FIFA-approved size and weight",
                        Price = 19.50m,
                        StockQuantity = 1000,
                        PhotoPath = "https://res.cloudinary.com/dpj4iudu1/image/upload/v1771279100/sportstore/products/jkmxfmo3vettiwuserzc.jpg",
                        CategoryId = categories["Soccer"].CategoryId
                    },
                    new Product
                    {
                        Name = "Corner Flags",
                        Description = "Give your playing field a professional touch",
                        Price = 34.95m,
                        StockQuantity = 1000,
                        PhotoPath = "https://res.cloudinary.com/dpj4iudu1/image/upload/v1771279990/sportstore/products/fpvswzpiahtbbvu8yase.jpg",
                        CategoryId = categories["Soccer"].CategoryId
                    },
                    new Product
                    {
                        Name = "Stadium",
                        Description = "Flat-packed 35,000-seat stadium",
                        Price = 79500,
                        StockQuantity = 1000,
                        PhotoPath = "https://res.cloudinary.com/dpj4iudu1/image/upload/v1771279919/sportstore/products/t54sqn3uwogmeyhbvfbs.jpg",
                        CategoryId = categories["Soccer"].CategoryId
                    },
                    new Product
                    {
                        Name = "Thinking Cap",
                        Description = "Improve brain efficiency by 75%",
                        Price = 16,
                        StockQuantity = 1000,
                        PhotoPath = "https://res.cloudinary.com/dpj4iudu1/image/upload/v1771279735/sportstore/products/vfhxhfr4goj2fodkn2dd.jpg",
                        CategoryId = categories["Chess"].CategoryId
                    },
                    new Product
                    {
                        Name = "Unsteady Chair",
                        Description = "Secretly give your opponent a disadvantage",
                        Price = 29.95m,
                        StockQuantity = 1000,
                        PhotoPath = "https://res.cloudinary.com/dpj4iudu1/image/upload/v1771279698/sportstore/products/eregkbuqz7ysamzbikbr.jpg",
                        CategoryId = categories["Chess"].CategoryId
                    },
                    new Product
                    {
                        Name = "Human Chess Board",
                        Description = "A fun game for the family",
                        Price = 75,
                        StockQuantity = 1000,
                        PhotoPath = "https://res.cloudinary.com/dpj4iudu1/image/upload/v1771279666/sportstore/products/ctguyzo3rbwvtwoxkkx8.jpg",
                        CategoryId = categories["Chess"].CategoryId
                    },
                    new Product
                    {
                        Name = "Roller Blades",
                        Description = "Flow with the wind",
                        Price = 1200m,
                        StockQuantity = 1000,
                        PhotoPath = "https://res.cloudinary.com/dpj4iudu1/image/upload/v1771284861/sportstore/products/updexwlu7obd5layveop.jpg",
                        CategoryId = categories["Action"].CategoryId
                    }
                );

                context.SaveChanges();
            }

        }
    }
}
