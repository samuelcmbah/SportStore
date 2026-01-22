// SportStore/Controllers/CartController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportStore.Models;
using SportStore.Services.IServices;
using SportStore.Utils;
using SportStore.ViewModels.CartVM;

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
            SessionCart sessionCart,
            ICartService cartService,
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

        /// <summary>
        /// Handles both adding new items and updating existing item quantities
        /// Used by: Product Details page dropdown, Cart page dropdown
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateCart(int productId, int quantity, string returnUrl)
        {
            Product? product = storeRepository.GetProductById(productId);
            if (product == null)
            {
                return NotFound();
            }

            var cart = await GetCartAsync();
            var existingItem = cart.CartItems
                .FirstOrDefault(i => i.Product.ProductID == productId);

            // Handle removal (quantity = 0)
            if (quantity == 0)
            {
                if (existingItem != null)
                {
                    cartDomainService.RemoveItem(cart, productId);
                    await SaveCartAsync(cart);
                    TempData["Success"] = "Item removed from cart";
                }
                return LocalRedirect(returnUrl);
            }

            // Validate quantity
            if (quantity < 0)
            {
                return BadRequest("Invalid quantity");
            }

            // Check stock availability
            if (!product.HasStock(quantity))
            {
                TempData["Error"] = $"Only {product.StockQuantity} items available in stock";
                return LocalRedirect(returnUrl);
            }

            // Update or add item
            if (existingItem != null)
            {
                existingItem.Quantity = quantity;
                TempData["Success"] = "Cart updated successfully";
            }
            else
            {
                cartDomainService.AddItem(cart, product, quantity);
                TempData["Success"] = "Item added to cart";
            }

            await SaveCartAsync(cart);
            return LocalRedirect(returnUrl);
        }

        /// <summary>
        /// Quick remove action for cart page
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId, string returnUrl)
        {
            var cart = await GetCartAsync();
            cartDomainService.RemoveItem(cart, productId);
            await SaveCartAsync(cart);

            TempData["Success"] = "Item removed from cart";
            return LocalRedirect(returnUrl);
        }

        /// <summary>
        /// Display cart page
        /// </summary>
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