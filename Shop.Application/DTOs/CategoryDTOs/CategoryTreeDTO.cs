namespace Shop.Application.DTOs.CategoryDTOs;

public class CategoryTreeDTO
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Slug { get; set; }

    public string? Url { get; set; }

    public bool IsActive { get; set; }

    public int? ParentId { get; set; }

    public List<CategoryTreeDTO> Children { get; set; } = new();
}