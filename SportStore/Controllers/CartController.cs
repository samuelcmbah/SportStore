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
        public IActionResult RemoveFromCart(int productId, string returnUrl)
        {
            Product? product = storeRepository.GetProductById(productId);
            if (product == null)
            {
                //Display an error message that the selected item is no longer available
            }

            var cart = HttpContext.Session.Get<CartViewModel>("Cart") ?? new CartViewModel();

            cart.CartItems.RemoveAll(id => id?.Product?.ProductID == product?.ProductID);

            CalculateTotalAmount_Items(cart);

            HttpContext.Session.Set("Cart", cart);

            return LocalRedirect(returnUrl);
        }

        private void CalculateTotalAmount_Items(CartViewModel cart)
        {
            cart.TotalCartItems = cart.CartItems.Sum(item => item?.Quantity);
            ViewData["TotalCartItems"] = cart.TotalCartItems.ToString();
            cart.Total = cart.CartItems.Sum(item => item.Subtotal);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, string returnUrl, int quantity = 1 )
        {
            Product? product = storeRepository.GetProductById(productId);
            if (product == null)
            {
                //Display an error message that the selected item is no longer available
            }
            
            //uses the SessionExtension.Get method to retrieve the current cart session, if it returns null, a new empty cart is created
            var cart = HttpContext.Session.Get<CartViewModel>("Cart") ?? new CartViewModel();

            {//checks if a product is already existing in the cart
            //increases the quantity by 1 if it already exists, adds it to the cart if it doesnt
            //assign the sum total of the quantity of each cart item to totalcartitems
            //and then calculates the total number of items in the cart
            }
            
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

            CalculateTotalAmount_Items(cart);

            //uses the SessionExtension.Set method to store the updated shopping cart in the session
            HttpContext.Session.Set("Cart", cart);

            return LocalRedirect(returnUrl);
        }

        public IActionResult ViewCart(string returnUrl)
        {
            var cart = HttpContext.Session.Get<CartViewModel>("Cart") ?? new CartViewModel();
            ViewBag.returnUrl = returnUrl;
            return View(cart);
        }
    }
}
