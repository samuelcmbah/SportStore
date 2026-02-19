using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using SportStore.Models;
using System.Text.Json.Serialization;

namespace SportStore.Utils
{

    public class SessionCart 
    {
        private const string CartSessionKey = "Cart";
        private ISession? _session;

        public SessionCart(IHttpContextAccessor httpContextAccessor)
        {
            _session = httpContextAccessor.HttpContext?.Session;
        }

        public Cart GetCart()
        {
            if (_session == null)
            {
                return new Cart();
            }
            var sessionData = _session.GetString(CartSessionKey);
            return sessionData == null ? new Cart() : (JsonConvert.DeserializeObject<Cart>(sessionData) ?? new Cart());
        }

        public void SetCart(Cart cart)
        {
            _session?.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
        }

        public void ClearCart()
        {
            _session?.Remove(CartSessionKey);
        }
    }
}
