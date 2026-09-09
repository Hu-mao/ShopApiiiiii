namespace Shop.Application.DTOs.CategoryDTOs;

public class CategoryPagedDTO
{
    public List<CategoryReadDTO> Items { get; set; } = [];
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}