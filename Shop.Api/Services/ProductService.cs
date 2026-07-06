using Shop.Api.Interfaces;
using Shop.Domain.Models;

namespace Shop.Api.Services;

public class ProductService : IProductService
{
    private List<Product> _products = new();
    public ProductService()
    {
        _products.Add(new Product()
        {
            Name = "Milk",
            Price = 40.9m
        });
        _products.Add(new Product()
        {
            Name = "Bread",
            Price = 30.5m
        });
    }

    public void AddProduct(Product product)
    {
        _products.Add(product);
    }

    public List<Product> GetAllProducts()
    {
       
        return _products;
    }
}
