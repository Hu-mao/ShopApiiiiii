using Shop.Domain.Models;

namespace Shop.Api.Interfaces;

public interface IProductService
{
    List<Product> GetAllProducts();
    void AddProduct(Product product);
}
