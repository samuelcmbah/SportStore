using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportStore.Models;
using SportStore.Models.ViewModels;
using System.Diagnostics;

namespace SportStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IStoreRepository storeRepository;
        public int ProductPerPage = 4;

        public HomeController(ILogger<HomeController> logger, IStoreRepository storeRepository)
        {
            this.logger = logger;
            this.storeRepository = storeRepository;
        }

        [AllowAnonymous]
        public IActionResult Index(string? category = null, int productPage = 1)
        {
            var model = new ProductsListViewModel
            {
                //chooses a category if one is indicated, orders the products, skips the products before the designated page and displays the next 4 products
                Products = storeRepository.AllProducts.Where(p => category == null || p.Category == category)
                .OrderBy(p => p.ProductID).Skip((productPage - 1) * ProductPerPage).Take(ProductPerPage),

                PagingInfo = new PagingInfoViewModel
                {
                    TotalItems = category == null ? storeRepository.AllProducts.Count() : storeRepository.AllProducts.Where(c => c.Category == category).Count(),
                    ItemsPerPage = ProductPerPage,
                    CurrentPage = productPage,
                },

                CurrentCategory = category
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
