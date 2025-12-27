

using SportStore.Models;
namespace SportStore.ViewModels;

public class CartItem
{
    public int CartItemID { get; set; }
    public Product Product { get; set; } = new();
    public int Quantity { get; set; }

    //foreign key and navigation prop
    public int CartId { get; set; }
    public Cart Cart { get; set; } = null!;

    public decimal Subtotal => Quantity * Product.Price;
}