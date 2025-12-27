namespace SportStore.Components
{
    using global::SportStore.Models;
    using global::SportStore.Services.IServices;
    using global::SportStore.Utils;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    using System.Threading.Tasks;

    namespace SportStore.Components
    {
        public class CartWidgetViewComponent : ViewComponent
        {
            private readonly ICartService _cartService;
            private readonly SessionCart _sessionCart;
            private readonly UserManager<ApplicationUser> _userManager;

            public CartWidgetViewComponent(
                ICartService cartService,
                SessionCart sessionCart,
                UserManager<ApplicationUser> userManager)
            {
                _cartService = cartService;
                _sessionCart = sessionCart;
                _userManager = userManager;
            }

            public async Task<IViewComponentResult> InvokeAsync()
            {
                int? totalItems = 0;

                if (User.Identity!.IsAuthenticated)
                {
                    var userId = _userManager.GetUserId(HttpContext.User);
                    var cart = await _cartService.GetOrCreateCartByUserIdAsync(userId);
                    totalItems = cart.TotalCartItems;
                }
                else
                {
                    var cart = _sessionCart.GetCart();
                    totalItems = cart.TotalCartItems;
                }

                return View(totalItems);
            }
        }
    }
}
