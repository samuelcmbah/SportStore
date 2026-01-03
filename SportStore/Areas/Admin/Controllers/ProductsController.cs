using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SportStore.Models;
using SportStore.Services.IServices;
using SportStore.ViewModels.ProductVM;

namespace SportStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class ProductsController : Controller
    {
        private readonly IStoreRepository storeRepository;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductsController(IStoreRepository storeRepository, IWebHostEnvironment webHostEnvironment)
        {
            this.storeRepository = storeRepository;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult DeleteProduct(long id)
        {
            Product? product = storeRepository.GetProductById(id);
            if (product == null)
            {
                //tell the admin that the product does not exist and return them to the avilable lisst of products
            }
            storeRepository.DeleteProduct(product);

            ProductsListViewModel model = new ProductsListViewModel()
            {
                Products = storeRepository.AllProducts
            };


            return View("ManageProducts", model);
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateProduct(ProductCreateViewModel model)
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

                storeRepository.CreateProduct(product);
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
        public IActionResult Edit(long id)
        {
            Product? product = storeRepository.GetProductById(id);
            if (product == null)
            {
                //direct to ProductNotFound page and then to the admin llst of all products
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
        public IActionResult Edit(ProductEditViewModel model)
        {

            if (ModelState.IsValid)
            {
                Product? editedProduct = storeRepository.GetProductById(model.ProductID);
                if (editedProduct != null)
                {
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
                }
                storeRepository.UpdateProduct(editedProduct);
                return RedirectToAction("Details", new { id = model.ProductID });
            }
            return View();
        }

        public IActionResult Details(long id)
        {
            Product? product = storeRepository.GetProductById(id);
            if (product == null)
            {
                //direct to ProductNotFound page and then to the admin list of all products
                Response.StatusCode = 404;
                return View("ProductNotFound");
            }

            return View(product);
        }

        public IActionResult ManageProducts()
        {
            var model = new ProductsListViewModel
            {
                Products = storeRepository.AllProducts
            };
            return View(model);
        }
    }
}
