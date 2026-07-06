using Shop.Application.DTOs.CategoryDTOs;

namespace Shop.Api.Request.Category
{
    public class CategoryCreateRequest : CategoryCreateDTO
    {
        public IFormFile? Image { get; set; }
    }
}
