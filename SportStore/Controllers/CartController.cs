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
                var userId = currentUserService.UserId!;
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

            // Check stock availability
            if (!product.HasStock(quantity))
            {
                TempData["Error"] = $"Only {product.StockQuantity} items available in stock";
                return LocalRedirect(returnUrl);
            }

            var cart = await GetCartAsync();

            // Check if adding this quantity would exceed stock
            var existingItem = cart.CartItems
                .FirstOrDefault(i => i.Product.ProductID == productId);

            int totalQuantity = (existingItem?.Quantity ?? 0) + quantity;

            if (!product.HasStock(totalQuantity))
            {
                TempData["Error"] = $"Cannot add {quantity} more. Only {product.StockQuantity} items available (you have {existingItem?.Quantity ?? 0} in cart)";
                return LocalRedirect(returnUrl);
            }

            cartDomainService.AddItem(cart, product, quantity);

            await SaveCartAsync(cart);

            return LocalRedirect(returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity, string returnUrl)
        {
            if (quantity < 1)
            {
                // If quantity is 0, remove the item
                return await RemoveFromCart(productId, returnUrl);
            }

            Product? product = storeRepository.GetProductById(productId);
            if (product == null)
            {
                return NotFound();
            }

            // Check stock availability
            if (!product.HasStock(quantity))
            {
                TempData["Error"] = $"Only {product.StockQuantity} items available in stock";
                return LocalRedirect(returnUrl);
            }

            var cart = await GetCartAsync();

            var existingItem = cart.CartItems
                .FirstOrDefault(i => i.Product.ProductID == productId);

            if (existingItem != null)
            {
                existingItem.Quantity = quantity;
                await SaveCartAsync(cart);
            }

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
