using Microsoft.EntityFrameworkCore;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories;

public class CategoryRepository(ShopDbContext _context) : ICategoryRepository
{
    public async Task<int?> AddCategoryAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return category.Id;
    }

    public async Task<List<Category>?> GetAllCategoriesAsync()
    {
        return await _context.Categories.ToListAsync();

    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<bool> UpdateCategoryAsync(Category category)
    {
        var entity = await _context.Categories.FindAsync(category.Id);

        if (entity == null)
            return false;

        entity.Name = category.Name;
        entity.Url = category.Url;
        entity.Slug = category.Slug;
        entity.ParentId = category.ParentId;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
            return false;

        _context.Categories.Remove(category);

        await _context.SaveChangesAsync();

        return true;
    }
}
