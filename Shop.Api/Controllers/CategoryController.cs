using Microsoft.AspNetCore.Mvc;
using Shop.Api.Interfaces;
using Shop.Api.Request.Category;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Services;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")] //https://ip:port/api/v1
public class CategoryController(ICategoryService _categoryService, IImageService _imageService):ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromForm] CategoryCreateRequest dto)
    {
        string? url = null;

        if (dto.Image != null)
        {
            url = await _imageService.SaveFileAsync(dto.Image, "categories");
        }

        var createDto = new CategoryCreateDTO
        {
            Name = dto.Name,
            Url = url,
            Slug = dto.Slug,
            ParentId = dto.ParentId,
        };

        var id = await _categoryService.CreateCategoryAsync(createDto);

        return Ok($"Category created {id}");
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        List<CategoryReadDTO>? categories = await _categoryService.GetAllCategoriesAsync();
        if(categories== null || categories.Count == 0)
        {
            return NotFound();
        }
        return Ok(categories);
    }
    [HttpPut]
    public async Task<IActionResult> UpdateCategory([FromBody] CategoryUpdateDTO dto)
    {
        var result = await _categoryService.UpdateCategoryAsync(dto);

        if (!result)
            return NotFound();

        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);

        if (!result)
            return NotFound();

        return NoContent();
    }
}
