using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SportStore.Models;
using SportStore.Services;
using SportStore.Services.IServices;
using SportStore.ViewModels.ProductVM;

namespace SportStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class ProductsController : Controller
    {
        private readonly IProductService productService;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductsController( IProductService productService, IWebHostEnvironment webHostEnvironment)
        {
            this.productService = productService;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult ManageProducts()
        {
            IEnumerable<Product> products = productService.GetAll().ToList()
                                                   ?? new List<Product>();

            return View(products);
        }

        public async Task<IActionResult> DeleteProduct(long id)
        {
            await productService.DeleteAsync(id);

            return RedirectToAction("ManageProducts");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string? uniqueFileName = UploadFile(model);
                //fix later
                Product product = new()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Category = model.Cartegory,
                    Price = model.Price,
                    PhotoPath = uniqueFileName
                };

                productService.CreateAsync(product);
                return RedirectToAction("Details", new { id = product.ProductID });
            }
            return View();
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

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var product = await productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ProductEditViewModel productEditViewModel = new()
            {
                ProductID = product.ProductID,
                Name = product.Name,
                Description = product.Description,
                Cartegory = product.Category,
                Price = product.Price,
                ExistingPhotoPath = product.PhotoPath,
            };

            return View(productEditViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(ProductEditViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            var editedProduct = await productService.GetByIdAsync(model.ProductID);

            if (editedProduct == null)
            {
                return NotFound();
            }

            editedProduct.Name = model.Name;
            editedProduct.Description = model.Description;
            editedProduct.Category = model.Cartegory;
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
            await productService.UpdateAsync(editedProduct);
            return RedirectToAction("Details", new { id = model.ProductID });
        }

        public async Task<IActionResult> DetailsAsync(long id)
        {
            var product = await productService.GetByIdAsync(id);
            if (product == null)
            {
                
                Response.StatusCode = 404;
                return View("ProductNotFound");
            }

            return View(product);
        }

        
    }
}
