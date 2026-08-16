using AutoMapper;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;
namespace Shop.Application.Services;

public class CategoryService(ICategoryRepository _repository, IMapper _mapper, ICachingService _cacheService) : ICategoryService
{
    public async Task<int?> CreateCategoryAsync(CategoryCreateDTO dto)
    {
        return await _repository.AddCategoryAsync(_mapper.Map<Category>(dto));
    }

    public async Task<CategoryReadDTO?> GetCategoryByIdAsync(int id)
    {
        var category = await _repository.GetCategoryByIdAsync(id);
        return category == null ? null : _mapper.Map<CategoryReadDTO>(category);
    }

    //public async Task<List<CategoryReadDTO>?> GetAllCategoriesAsync()
    //{
    //    List<Category>? categories = await _repository.GetAllCategoriesAsync();

    //    if (categories == null || !categories.Any())
    //        return null;

    //    return _mapper.Map<List<CategoryReadDTO>>(categories);
    //}
    public async Task<List<CategoryReadDTO>?> GetAllCategoriesAsync()
    {
        const string cacheKey = "Categories";

        var cachedCategories =
            await _cacheService.GetAsync<List<CategoryReadDTO>>(cacheKey);

        if (cachedCategories != null)
        {
            return cachedCategories;
        }

        var categories =
            await _repository.GetAllCategoriesAsync();

        var categoryDTOs =
            _mapper.Map<List<CategoryReadDTO>>(categories);

        await _cacheService.SetAsync(
            cacheKey,
            categoryDTOs,
            TimeSpan.FromMinutes(15));

        return categoryDTOs;
    }
    public async Task<bool> UpdateCategoryAsync(CategoryUpdateDTO dto)
    {
        var category = _mapper.Map<Category>(dto);

        var result = await _repository.UpdateCategoryAsync(category);

        if (!result)
        {
            return false;
        }

        await _cacheService.RemoveAsync("Categories");
        await _cacheService.RemoveAsync($"Category:{category.Id}");

        return true;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var result = await _repository.DeleteCategoryAsync(id);

        if (!result)
        {
            return false;
        }

        await _cacheService.RemoveAsync("Categories");
        await _cacheService.RemoveAsync($"Category:{id}");

        return true;
    }
    public async Task<List<CategoryReadDTO>?> GetParentCategoriesAsync(int id)
    {
        var categories = await _repository.GetAllCategoriesAsync();

        if (categories == null)
            return null;

        var category = categories.FirstOrDefault(x => x.Id == id);

        if (category == null)
            return null;

        var result = new List<Category>();

        var current = category;

        while (current.ParentId != null)
        {
            var parent = categories.FirstOrDefault(
                x => x.Id == current.ParentId);

            if (parent == null)
                break;

            result.Add(parent);

            current = parent;
        }

        result.Reverse();

        return _mapper.Map<List<CategoryReadDTO>>(result);
    }
    public async Task<List<CategoryReadDTO>?> GetChildCategoriesAsync(int id)
    {
        var categories = await _repository.GetAllCategoriesAsync();

        if (categories == null)
            return null;

        var category = categories.FirstOrDefault(x => x.Id == id);

        if (category == null)
            return null;

        var result = new List<Category>();

        void AddChildren(int parentId)
        {
            var children = categories
                .Where(x => x.ParentId == parentId)
                .ToList();

            foreach (var child in children)
            {
                result.Add(child);

                AddChildren(child.Id);
            }
        }

        AddChildren(category.Id);

        return _mapper.Map<List<CategoryReadDTO>>(result);
    }
    public async Task<List<CategoryTreeDTO>> GetCategoryTreeAsync()
    {
        var categories = await _repository.GetAllCategoriesAsync();

        if (categories == null || !categories.Any())
            return new List<CategoryTreeDTO>();

        List<CategoryTreeDTO> BuildTree(int? parentId)
        {
            return categories
                .Where(x => x.ParentId == parentId)
                .Select(x => new CategoryTreeDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    Slug = x.Slug,
                    Url = x.Url,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,

                    Children = BuildTree(x.Id)
                })
                .ToList();
        }

        return BuildTree(null);
    }
}
