using SportStore.Models;
using SportStore.Models.Enums;
using SportStore.Services.IServices;
using SportStore.ViewModels;

namespace SportStore.Services
{
    public class OrderDomainService : IOrderDomainService
    {
        private readonly ICartDomainService _cartDomain;

        public OrderDomainService(ICartDomainService cartDomain)
        {
            _cartDomain = cartDomain;
        }

        public Order CreateOrderFromCart(Cart cart, CheckoutViewModel vm, string userId)
        {
            return new Order
            {
                OrderReference = $"ORD_{Guid.NewGuid():N}",
                Status = OrderStatus.Pending,
                OrderDate = DateTime.UtcNow,
                ShippedDate = new DateTime(),
                UserId = userId,
                Name = vm.Name,
                Email = vm.Email,
                Address = vm.Address,
                City = vm.City,
                State = vm.State,
                Zip = vm.Zip,
                Country = vm.Country,
                GiftWrap = vm.GiftWrap,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    Product = ci.Product,
                    Quantity = ci.Quantity
                }).ToList()
            };
        }
    }

}
