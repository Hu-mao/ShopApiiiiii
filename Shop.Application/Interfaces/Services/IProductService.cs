using System;
using System.Collections.Generic;
using System.Text;
using Shop.Application.DTOs.ProductDTOs;

namespace Shop.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<int> CreateAsync(ProductCreateDTO dto);

        Task<List<ProductReadDTO>> GetAllAsync();

        Task<ProductReadDTO?> GetByIdAsync(int id);
    }
}