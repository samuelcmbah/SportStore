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
            private readonly ICartDomainService cartDomainService;

            public CartWidgetViewComponent(
                ICartService cartService,
                SessionCart sessionCart,
                UserManager<ApplicationUser> userManager,
                ICartDomainService cartDomainService)
            {
                _cartService = cartService;
                _sessionCart = sessionCart;
                _userManager = userManager;
                this.cartDomainService = cartDomainService;
            }

            public async Task<IViewComponentResult> InvokeAsync()
            {
                Cart cart;
                int? totalItems = 0;
                var userId = _userManager.GetUserId(HttpContext.User);

                if (!string.IsNullOrEmpty(userId))
                {
                    
                    cart = await _cartService.GetOrCreateCartByUserIdAsync(userId);
                }
                else
                {
                    cart = _sessionCart.GetCart();
                }
                totalItems = cartDomainService.GetTotalItems(cart);

                return View(totalItems);
            }
        }
    }
}
