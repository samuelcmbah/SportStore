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
        private readonly ICloudinaryService cloudinaryService;
        private readonly ILogger<ProductService> logger;

        public ProductService(
            StoreDbContext context,
            IWebHostEnvironment webHostEnvironment,
            ICloudinaryService cloudinaryService,
            ILogger<ProductService> logger
        )
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
            this.cloudinaryService = cloudinaryService;
            this.logger = logger;
        }

        private async Task<string?> UploadImageAsync(IFormFile? photo)
        {
            if (photo == null)
            {
                return null;
            }

            try
            {
                var uploadResult = await cloudinaryService.UploadImageAsync(photo);

                if (uploadResult.Error != null)
                {
                    logger.LogError("Cloudinary upload error: {ErrorMessage}", uploadResult.Error.Message);
                    return null;
                }

                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception during image upload: {ExceptionMessage}", ex.Message);
                return null;
            }
        }

        private async Task DeleteImageAsync(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return;

            try
            {
                var publicId = cloudinaryService.ExtractPublicIdFromUrl(imageUrl);

                if (!string.IsNullOrEmpty(publicId))
                {
                    await cloudinaryService.DeleteImageAsync(publicId);
                    logger.LogInformation("Deleted image with public ID: {PublicId}", publicId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception during image deletion: {ExceptionMessage}", ex.Message);
            }
        }



        public IQueryable<Product> SearchFilter(ProductSearchFilterQuery query)
        {
            IQueryable<Product> products = context.Products
                    .Include(p => p.Category);

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                string term = query.SearchTerm.Trim().ToLower();

                products = products.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    p.Description.ToLower().Contains(term) ||
                    p.Category.Name.ToLower().Contains(term));
            }

            if (query.CategoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == query.CategoryId.Value);
            }

            if (query.MinPrice.HasValue)
            {
                products = products.Where(p => p.Price >= query.MinPrice.Value);
            }

            if (query.MaxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= query.MaxPrice.Value);
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
            string? imageUrl = await UploadImageAsync(model.Photo);

            Product product = new()
            {
                Name = model.Name,
                Description = model.Description,
                CategoryId = model.CategoryId,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                PhotoPath = imageUrl //stores full cloudinary url
            };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            logger.LogInformation("Created new product with ID: {ProductId}", product.ProductID);
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
            editedProduct.StockQuantity = model.StockQuantity;

            if (model.Photo != null)
            {
                // Delete old image from Cloudinary
                await DeleteImageAsync(editedProduct.PhotoPath);

                // Upload new image
                editedProduct.PhotoPath = await UploadImageAsync(model.Photo);
            }

            context.Products.Update(editedProduct);
            await context.SaveChangesAsync();
            return editedProduct;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var product = await context.Products.FindAsync(id);

            if (product == null) return false;

            // Delete image from Cloudinary before deleting product
            await DeleteImageAsync(product.PhotoPath);

            context.Products.Remove(product);
            await context.SaveChangesAsync();

            logger.LogInformation("Deleted product {id}", id);
            return true;
        }
    }
}
