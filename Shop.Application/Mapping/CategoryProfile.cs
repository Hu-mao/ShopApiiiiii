using AutoMapper;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Mapping
{
    public class CategoryProfile:Profile
    {
        public CategoryProfile()
        {
            CreateMap<CategoryCreateDTO, Category>();
            CreateMap<Category, CategoryReadDTO>()
                .ForMember
                (dest => dest.Products,
                opt => opt.MapFrom(src => src.Products.Select(p => p.Id).ToList()));
        }
    }
}
