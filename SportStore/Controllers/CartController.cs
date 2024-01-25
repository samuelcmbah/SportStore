using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportStore.Models;
using SportStore.Models.ViewModels;
//using Microsoft.AspNetCore.Mvc;
//using Web.Extensions;


namespace SportStore.Controllers
{
    public class CartController : Controller
    {
        private readonly IStoreRepository storeRepository;

        public CartController(IStoreRepository storeRepository)
        {
            this.storeRepository = storeRepository;
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            Product? product = storeRepository.GetProductById(productId);
            if (product == null)
            {
                //Display an error message that the selected item is no longer available
            }
            
            //uses the SessionExtension.Get method to retrieve the current cart session, if it returns null, a new empty cart is created
            var cart = HttpContext.Session.Get<CartViewModel>("Cart") ?? new CartViewModel();

            //checks if a product is already existing in the cart
            //increases the quantity by 1 if it already exists, adds it to the cart if it doesnt
            //assign the sum total of the quantity of each cart item to totalcartitems
            //and then calculates the total number of items in the cart
            var existingCartItem = cart.CartItems.FirstOrDefault(item => item?.Product?.ProductID == productId);
            if (existingCartItem != null)
            {
                existingCartItem.Quantity += 1;
            }
            else
            {
                var cartItem = new CartItem
                {
                    Product = product,
                    Quantity = quantity
                };
                cart.CartItems.Add(cartItem);
            }

            cart.TotalCartItems = cart.CartItems.Sum(item => item?.Quantity);
            cart.Total = cart.CartItems.Sum(item => item.Subtotal);

            //uses the SessionExtension.Set method to store the updated shopping cart in the session
            HttpContext.Session.Set("Cart", cart);

            return RedirectToAction("index", "home");
        }

        public IActionResult ViewCart()
        {
            var cart = HttpContext.Session.Get<CartViewModel>("Cart") ?? new CartViewModel();
            return View(cart);
        }
    }
}
