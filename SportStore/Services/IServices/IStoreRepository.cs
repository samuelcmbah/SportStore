using SportStore.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SportStore.Services.IServices
{
    public interface IStoreRepository
    {
        //a property that returns an IQueryable<Product>,
        //allowing for flexible querying of product data.
        IQueryable<Product> AllProducts { get; }
        IQueryable<Category> Categories { get; }

        Product? GetProductById(long? id);
        Product? CreateProduct(Product product);
        Product? UpdateProduct(Product? editedProduct);
        Product? DeleteProduct(Product? product);
    }
}
