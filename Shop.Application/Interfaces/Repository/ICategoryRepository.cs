using Shop.Application.DTOs.CategoryDTOs;
using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Repository;

public interface ICategoryRepository
{
    Task<int?> AddCategoryAsync(Category category);
    Task<CategoryReadDTO?> GetCategoryByIdAsync(int id);
    Task<List<Category>?> GetAllCategoriesAsync();
}
