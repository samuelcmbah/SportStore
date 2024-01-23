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
            //check if cartItem is already in cartitems
            //if true assess the already existing cartitem quantity and increase it by 1
            //assign the sum total of the quantity of each cart item to totalcartitems
            //else just add the cartitem to the list
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
