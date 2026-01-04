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

        public async Task<IActionResult> Delete(long id)
        {
            await productService.DeleteAsync(id);

            return RedirectToAction("Index");
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
            
           var product = await productService.CreateAsync(model);

            if(product == null)
            {
                return View("Error");
            }

            return RedirectToAction(nameof(Index));

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

           
            await productService.UpdateAsync(model);
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
