using Shop.Application.DTOs.CategoryDTOs;


namespace Shop.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<int?> CreateCategoryAsync(CategoryCreateDTO dto);
    Task<CategoryReadDTO?> GetCategoryByIdAsync(int id);
    Task<List<CategoryReadDTO>?> GetAllCategoriesAsync();
}
