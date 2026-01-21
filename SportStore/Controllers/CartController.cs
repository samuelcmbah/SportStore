using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportStore.Models;
using SportStore.Services;
using SportStore.Services.IServices;
using SportStore.Utils;
using SportStore.ViewModels;
using SportStore.ViewModels.CartVM;
using System.Security.Claims;
using System.Threading.Tasks;

//using Microsoft.AspNetCore.Mvc;
//using Web.Extensions;


namespace SportStore.Controllers
{
    [AllowAnonymous]
    public class CartController : Controller
    {
        private readonly IStoreRepository storeRepository;
        private readonly SessionCart sessionCart;
        private readonly ICartService cartService;
        private readonly ICartDomainService cartDomainService;
        private readonly ICurrentUserService currentUserService;

        public CartController(
            IStoreRepository storeRepository, 
            SessionCart sessionCart, ICartService cartService,
            ICartDomainService cartDomainService,
            ICurrentUserService currentUserService)
        {
            this.storeRepository = storeRepository;
            this.sessionCart = sessionCart;
            this.cartService = cartService;
            this.cartDomainService = cartDomainService;
            this.currentUserService = currentUserService;
        }

        //PRIVATE HELPER METHODS



        private async Task<Cart> GetCartAsync()
        {
            if (User.Identity!.IsAuthenticated)
            {
                var userId = currentUserService.UserId;
                return await cartService.GetOrCreateCartByUserIdAsync(userId);
            }  
            
            return sessionCart.GetCart();
        }

        private async Task SaveCartAsync(Cart cart)
        {
            if (User.Identity!.IsAuthenticated)
                await cartService.UpdateCartAsync(cart);
            else
                sessionCart.SetCart(cart);
        }

        //PUBLIC ACCESS METHODS

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, string returnUrl, int quantity = 1)
        {
            Product? product = storeRepository.GetProductById(productId);
            if (product == null)
            {
                return NotFound();
            }

            var cart = await GetCartAsync();

            cartDomainService.AddItem(cart, product, quantity);

            await SaveCartAsync(cart);

            return LocalRedirect(returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId, string returnUrl)
        {
            Product? product = storeRepository.GetProductById(productId);
            if (product == null)
            {
                return NotFound();
            }

            var cart = await GetCartAsync();

            cartDomainService.RemoveItem(cart, product.ProductID);

            await SaveCartAsync(cart);

            return LocalRedirect(returnUrl);
        }

        public async Task<IActionResult> ViewCart()
        {
            var cart = await GetCartAsync();

            var viewModel = new CartViewModel
            {
                Cart = cart,
                TotalItems = cartDomainService.GetTotalItems(cart),
                TotalPrice = cartDomainService.GetTotalPrice(cart),
            };

            return View(viewModel);
        }

    }
}
