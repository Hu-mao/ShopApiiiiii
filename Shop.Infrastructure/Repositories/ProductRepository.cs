using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories;

public class ProductRepository(ShopDbContext _context) : IProductRepository
{
    public async Task<int> CreateAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        return product.Id;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Images)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}