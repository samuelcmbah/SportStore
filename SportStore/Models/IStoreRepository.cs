using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SportStore.Models
{
    public interface IStoreRepository
    {
        //a property that returns an IQueryable<Product>,
        //allowing for flexible querying of product data.
        IQueryable<Product> AllProducts { get; }
        Product? GetProductById(long id);
    }
}
