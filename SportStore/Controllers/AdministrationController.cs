using Microsoft.AspNetCore.Mvc;
using SportStore.Models;
using SportStore.Models.ViewModels;

namespace SportStore.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly IStoreRepository storeRepository;
        private readonly IOrderRepository orderRepository;

        public AdministrationController(IStoreRepository storeRepository, IOrderRepository orderRepository)
        {
            this.storeRepository = storeRepository;
            this.orderRepository = orderRepository;
        }

        public IActionResult DeleteProduct(long id)
        {
            Product? product = storeRepository.GetProductById(id);
            if(product == null)
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
                Product product = new()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Category = model.Cartegory,
                    Price = model.Price
                };

                storeRepository.CreateProduct(product);
                return RedirectToAction("Details", new { id = product.ProductID });
            }
            return View();
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
                Price = product.Price
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
                }
                storeRepository.UpdateProduct(editedProduct);
                return RedirectToAction("Details", new {id = model.ProductID});
            }
            return View();
        }

        public IActionResult Details(long id)
        {
           Product? product = storeRepository.GetProductById(id);
            if (product == null)
            {
                //direct to ProductNotFound page and then to the admin llst of all products
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

        public IActionResult ManageOrders(int orderId)
        {
            if(orderId != 0)
            {
                Order order = orderRepository.GetOrder(orderId);
                if (order == null)
                {
                    //take the user to a NotFound page
                }
                order.Shipped = true;
                orderRepository.SaveOrder(order);
            }

            var model = orderRepository.Orders.Where(o => !o.Shipped).ToList() ;
            return View(model);
        }

        public IActionResult ManageShippedOrders()
        {
            var model = orderRepository.Orders.Where(o => o.Shipped).ToList();
            return View(model);
        }
    }
}
