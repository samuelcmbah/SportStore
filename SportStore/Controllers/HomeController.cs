using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportStore.Models;
using SportStore.Services;
using SportStore.Services.IServices;
using SportStore.Utils;
using SportStore.ViewModels;
using SportStore.ViewModels.ProductVM;

namespace SportStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IProductService productService;
        private readonly ICartService cartService;
        private readonly SessionCart sessionCart;
        private readonly ICurrentUserService currentUserService;
        public int ProductPerPage = 8;

        public HomeController(ILogger<HomeController> logger, 
            IProductService productService,
            ICartService cartService,
            SessionCart sessionCart,
            ICurrentUserService currentUserService)
        {
            this.logger = logger;
            this.productService = productService;
            this.cartService = cartService;
            this.sessionCart = sessionCart;
            this.currentUserService = currentUserService;
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

            Cart cart;
            if (currentUserService.IsAuthenticated)
            {
                cart = await cartService.GetOrCreateCartByUserIdAsync(currentUserService.UserId!);
            }
            else
            {
                cart = sessionCart.GetCart();
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Product.ProductID == id);


            var vm = new ProductDetailsViewModel
            {
                Product = product,
                IsInCart = cartItem != null,
                QuantityInCart = cartItem?.Quantity ?? 0,
                MaxQuantity = Math.Min(product.StockQuantity, 10)
            };
            return View(vm);
        }

       
    }
}
