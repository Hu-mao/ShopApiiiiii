using Shop.Application.DTOs.CategoryDTOs;
using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Repository;

public interface ICategoryRepository
{
    Task<int?> AddCategoryAsync(Category category);
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<List<Category>?> GetAllCategoriesAsync();
    Task<bool> UpdateCategoryAsync(Category category);
    Task<bool> DeleteCategoryAsync(int id);
}
