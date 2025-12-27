using Microsoft.EntityFrameworkCore;
using SportStore.Data;
using SportStore.Models;
using SportStore.Services.IServices;
using SportStore.ViewModels;

namespace SportStore.Services
{
    public class CartService : ICartService
    {
        private readonly StoreDbContext _context;

        public CartService(StoreDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetOrCreateCartByUserIdAsync(string userId)
        {
            var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null) 
            {
                return cart;
            }
                

            cart = new Cart
            {
                UserId = userId,
                CartItems = new List<CartItem>()
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            return cart;
        }

        public async Task UpdateCartAsync(Cart cart)
        {
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
        }

        public async Task MergeCartsAsync(string userId, Cart sessionCart)
        {
            var dbCart = await GetOrCreateCartByUserIdAsync(userId);

            foreach (var sessionItem in sessionCart.CartItems)
            {
                var dbItem = dbCart.CartItems
                    .FirstOrDefault(i => i.Product.ProductID == sessionItem.Product.ProductID);

                if (dbItem != null)
                {
                    dbItem.Quantity += sessionItem.Quantity;
                }
                else
                {
                    dbCart.CartItems.Add(new CartItem
                    {
                        Product = sessionItem.Product,
                        Quantity = sessionItem.Quantity
                    });
                }
            }

            await UpdateCartAsync(dbCart);
        }
    }

}
