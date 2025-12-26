using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using SportStore.Models;
using System.Text.Json.Serialization;

namespace SportStore.Utils
{

    public class SessionCart 
    {
        private const string CartSessionKey = "Cart";
        private ISession _session;

        public SessionCart(ISession session)
        {
            _session = session;
        }

        public Cart GetCart()
        {
            var sessionData = _session.GetString(CartSessionKey);
            return sessionData == null ? new Cart() : JsonConvert.DeserializeObject<Cart>(sessionData);
        }

        public void SetCart(Cart cart)
        {
            _session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
        }

        public void RemoveCart()
        {
            _session.Remove(CartSessionKey);
        }
    }
}
