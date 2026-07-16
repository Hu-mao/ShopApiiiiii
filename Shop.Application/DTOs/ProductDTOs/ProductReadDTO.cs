using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.DTOs.ProductDTOs
{
    public class ProductReadDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int StockQty { get; set; }

        public bool IsActive { get; set; }

        public int CategoryId { get; set; }

        public List<string> Images { get; set; } = new();
    }
}
