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
        public int ProductPerPage = 8;

        public HomeController(ILogger<HomeController> logger, IProductService productService )
        {
            this.logger = logger;
            this.productService = productService;
        }

        [AllowAnonymous]
        public IActionResult Index(
            string? search, 
            int? categoryId, 
            decimal? minPrice,
            decimal? maxPrice, 
            int productPage = 1)
        {
            var query = new ProductSearchFilterQuery
            {
                SearchTerm = search,
                CategoryId = categoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            var productsQuery = productService.SearchFilter(query) ;

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


        public async Task<IActionResult> Details(long id)
        {
            
            var product = await productService.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


    }
}
