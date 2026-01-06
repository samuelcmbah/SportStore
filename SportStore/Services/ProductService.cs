using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using SportStore.Data;
using SportStore.Models;
using SportStore.Services.IServices;
using SportStore.ViewModels;
using SportStore.ViewModels.ProductVM;

namespace SportStore.Services
{
    public class ProductService : IProductService
    {
        private readonly StoreDbContext context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductService(
                    StoreDbContext context,
                    IWebHostEnvironment webHostEnvironment
        )
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        private string? UploadFile(ProductCreateViewModel model)
        {
            string? uniqueFileName = null;

            if (model.Photo != null)
            {
                //create the upload path
                var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                //create unique file name
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                //create file path
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                //copy to images folder
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }

        public IQueryable<Product> Search(ProductSearchQuery query)
        {
            IQueryable<Product> products = context.Products
                    .Include(p => p.Category);

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                string term = query.SearchTerm.Trim();

                products = products.Where(p => 
                    p.Name.Contains(term) || p.Description.Contains(term));
            }

            if (query.CategoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == query.CategoryId.Value);
            }

            //if (!query.IncludeInactive)
            //{
            //    products = products.Where(p => p.IsActive);
            //}

            return products;
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

        public async Task<Product> CreateAsync(ProductCreateViewModel model)
        {
            string? uniqueFileName = UploadFile(model);

            Product product = new()
            {
                Name = model.Name,
                Description = model.Description,
                CategoryId = model.CategoryId,
                Price = model.Price,
                PhotoPath = uniqueFileName
            };
            context.Products.Add(product);
            await context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(ProductEditViewModel model)
        {
            var editedProduct = await GetByIdAsync(model.ProductID);

            if (editedProduct == null)
            {
                throw new InvalidOperationException("Product not found");
            }

            editedProduct.Name = model.Name;
            editedProduct.Description = model.Description;
            editedProduct.CategoryId = model.CategoryId;
            editedProduct.Price = model.Price;

            if (model.Photo != null)
            {
                if (model.ExistingPhotoPath != null)
                {
                    string filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                    System.IO.File.Delete(filePath);
                }
                editedProduct.PhotoPath = UploadFile(model);
            }

            context.Products.Update(editedProduct);
            await context.SaveChangesAsync();
            return editedProduct;
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
