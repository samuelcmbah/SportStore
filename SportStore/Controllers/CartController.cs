using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportStore.Models;
using SportStore.Services;
using SportStore.Services.IServices;
using SportStore.Utils;
using SportStore.ViewModels;
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

        public CartController(
            IStoreRepository storeRepository, 
            SessionCart sessionCart, ICartService cartService)
        {
            this.storeRepository = storeRepository;
            this.sessionCart = sessionCart;
            this.cartService = cartService;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";


        private async Task<Cart> GetCartAsync()
        {
            if(User.Identity!.IsAuthenticated)
            {
                var userId = GetUserId();
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

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId, string returnUrl)
        {
            Product? product = storeRepository.GetProductById(productId);
            if (product == null)
            {
                return NotFound();
            }

            var cart = await GetCartAsync();

            var itemToRemove = cart.CartItems.FirstOrDefault(item => item?.Product?.ProductID == productId);
            if (itemToRemove != null)
            {
                cart.CartItems.Remove(itemToRemove);
            }

            CalculateTotalAmount_Items(cart);

            await SaveCartAsync(cart);

            return LocalRedirect(returnUrl);
        }

        private void CalculateTotalAmount_Items(Cart cart)
        {
            cart.TotalCartItems = cart.CartItems.Sum(item => item?.Quantity);
            if(cart.TotalCartItems == 0)
            {
                cart.TotalCartItems = null;
            }
            ViewData["TotalCartItems"] = cart.TotalCartItems.ToString();
            cart.Total = cart.CartItems.Sum(item => item.Subtotal);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, string returnUrl, int quantity = 1 )
        {
            Product? product = storeRepository.GetProductById(productId);
            if (product == null)
            {
                return NotFound();
            }

            var cart = await GetCartAsync();

            var existingCartItem = cart.CartItems.FirstOrDefault(item => item?.Product?.ProductID == productId);
            if (existingCartItem != null)
            {
                existingCartItem.Quantity++;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    Product = product,
                    Quantity = quantity,
                    //Subtotal = product.Price * quantity | we are already doing this in the class definition
                });
            }

            CalculateTotalAmount_Items(cart);

            await SaveCartAsync(cart);

            return LocalRedirect(returnUrl);
        }


        public async Task<IActionResult> ViewCart()
        {
            var cart = await GetCartAsync();

            return View(cart);
        }

        //svdasffggf
    }
}
