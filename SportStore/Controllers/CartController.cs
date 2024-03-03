using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportStore.Models;
using SportStore.Models.ViewModels;

//using Microsoft.AspNetCore.Mvc;
//using Web.Extensions;


namespace SportStore.Controllers
{
    [AllowAnonymous]
    public class CartController : Controller
    {
        private readonly IStoreRepository storeRepository;
        private readonly SessionCart sessionCart;

        public CartController(IStoreRepository storeRepository, SessionCart sessionCart)
        {
            this.storeRepository = storeRepository;
            this.sessionCart = sessionCart;
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId, string returnUrl)
        {
            Product? product = storeRepository.GetProductById(productId);
            if (product == null)
            {
                //Display an error message that the selected item is no longer available
            }

            var cart = sessionCart.GetCart();

            cart.CartItems.RemoveAll(id => id?.Product?.ProductID == product?.ProductID);
            CalculateTotalAmount_Items(cart);

            sessionCart.SetCart(cart); 

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
        public IActionResult AddToCart(int productId, string returnUrl, int quantity = 1 )
        {
            Product? product = storeRepository.GetProductById(productId);
            if (product == null)
            {
                //Display an error message that the selected item is no longer available
            }

            var cart = sessionCart.GetCart();

            {
             //checks if a product is already existing in the cart
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

            sessionCart.SetCart(cart);

            return LocalRedirect(returnUrl);
        }


        public IActionResult ViewCart()
        {
            var cart = sessionCart.GetCart();

            return View(cart);
        }

        //svdasffggf
    }
}
