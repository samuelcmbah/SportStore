using SportStore.Models;
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

        public Order CreateOrderFromCart(Cart cart, CheckoutViewModel vm, string? userId)
        {
            return new Order
            {
                OrderDate = DateTime.UtcNow,
                ShippedDate = new DateTime(),
                UserId = userId,
                Name = vm.Name,
                Email = vm.Email,
                Line1 = vm.Line1,
                Line2 = vm.Line2,
                Line3 = vm.Line3,
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
