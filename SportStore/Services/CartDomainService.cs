using SportStore.Models;
using SportStore.Services.IServices;
using SportStore.ViewModels.CartVM;

namespace SportStore.Services
{
    public class CartDomainService : ICartDomainService
    {
        public void AddItem(Cart cart, Product product, int quantity = 1)
        {
            var item = cart.CartItems
                .FirstOrDefault(i => i.Product.ProductID == product.ProductID);
            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    Product = product,
                    Quantity = quantity
                });
            }
        }

        public void RemoveItem(Cart cart, long productId)
        {
            var item = cart.CartItems
                .FirstOrDefault(i => i.Product.ProductID == productId);

            if (item != null)
            {
                cart.CartItems.Remove(item);
            }
        }

        public int GetTotalItems(Cart cart)
        {
            return cart.CartItems.Sum(i => i.Quantity);
        }

        public decimal GetTotalPrice(Cart cart)
        {
            return cart.CartItems.Sum(i => i.Subtotal);
        }

        public void Merge(Cart targetCart, Cart sourceCart)
        {
            foreach (var sourceItem in sourceCart.CartItems)
            {
                AddItem(targetCart, sourceItem.Product, sourceItem.Quantity);
            }
        }
    }
}
