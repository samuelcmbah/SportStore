using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly ICategoryService categoryService;

        public ProductsController(
            IProductService productService, 
            IWebHostEnvironment webHostEnvironment,
            ICategoryService categoryService)
        {
            this.productService = productService;
            this.webHostEnvironment = webHostEnvironment;
            this.categoryService = categoryService;
        }
        public IActionResult Index()
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
            var model = new ProductCreateViewModel
            {
                //populate categories selectlist item
                Categories = categoryService.GetAll()
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.Name
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = categoryService.GetAll()
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.Name
                    });

                return View(model);
            }
            string? uniqueFileName = UploadFile(model);

            Product product = new()
            {
                Name = model.Name,
                Description = model.Description,
                CategoryId = model.CategoryId,
                Price = model.Price,
                PhotoPath = uniqueFileName
            };

            await productService.CreateAsync(product);

            return RedirectToAction(nameof(Index));

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
                Price = product.Price,
                ExistingPhotoPath = product.PhotoPath,
                CategoryId = product.CategoryId,
                Categories = categoryService.GetAll()
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.Name
                    })
                    .ToList()
            };

            return View(productEditViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(ProductEditViewModel model)
        {

            if (!ModelState.IsValid)
            {
                model.Categories = categoryService.GetAll()
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.Name
                    });

                return View(model);
            }

            var editedProduct = await productService.GetByIdAsync(model.ProductID);

            if (editedProduct == null)
            {
                return NotFound();
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
