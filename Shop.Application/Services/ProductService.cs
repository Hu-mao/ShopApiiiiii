using AutoMapper;
using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;

namespace Shop.Application.Services;

public class ProductService(
    IProductRepository _repository,
    IMapper _mapper)
    : IProductService
{
    public async Task<int> CreateAsync(ProductCreateDTO dto)
    {
        var product = _mapper.Map<Product>(dto);

        product.Images = dto.Images
            .Select((url, index) => new ProductImage
            {
                Url = url,
                IsPrimary = index == 0
            })
            .ToList();

        return await _repository.CreateAsync(product);
    }

    public async Task<List<ProductReadDTO>> GetAllAsync()
    {
        var products = await _repository.GetAllAsync();

        return products.Select(x => new ProductReadDTO
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Price = x.Price,
            StockQty = x.StockQty,
            IsActive = x.IsActive,
            CategoryId = x.CategoryId,
            Images = x.Images.Select(i => i.Url).ToList()
        }).ToList();
    }

    public async Task<ProductReadDTO?> GetByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);

        if (product == null)
            return null;

        return new ProductReadDTO
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQty = product.StockQty,
            IsActive = product.IsActive,
            CategoryId = product.CategoryId,
            Images = product.Images.Select(i => i.Url).ToList()
        };
    }
}
