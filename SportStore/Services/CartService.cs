using Microsoft.EntityFrameworkCore;
using SportStore.Data;
using SportStore.Models;
using SportStore.Services.IServices;
using SportStore.ViewModels.CartVM;

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
            await _context.SaveChangesAsync();
        }

       
    }

}
