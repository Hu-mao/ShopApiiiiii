using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.DTOs.CategoryDTOs;

public class CategoryReadDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public int? ParentId { get; set; } = null;
}
