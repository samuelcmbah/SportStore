using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportStore.Models;
using SportStore.Services.IServices;
using SportStore.ViewModels;
using SportStore.ViewModels.ProductVM;

namespace SportStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IProductService productService;
        public int ProductPerPage = 4;

        public HomeController(ILogger<HomeController> logger, IProductService productService )
        {
            this.logger = logger;
            this.productService = productService;
        }

        [AllowAnonymous]
        public IActionResult Index(string? search, int? categoryId, int productPage = 1)
        {
            var query = new ProductSearchQuery
            {
                SearchTerm = search,
                CategoryId = categoryId
            };

            var productsQuery = productService.Search(query) ;

            var model = new ProductsListViewModel
            {
                Products = productsQuery
                    .OrderBy(p => p.ProductID)
                    .Skip((productPage - 1) * ProductPerPage)
                    .Take(ProductPerPage),

                PagingInfo = new PagingInfoViewModel
                {
                    TotalItems = productsQuery.Count(),
                    ItemsPerPage = ProductPerPage,
                    CurrentPage = productPage
                },

                CurrentCategoryId = categoryId
            };

            return View(model);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        
    }
}
