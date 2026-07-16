using AutoMapper;
using Shop.Application.DTOs.UserDTOs;
using Shop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserCreateDTO, User>();
            CreateMap<User, UserReadDTO>();
            CreateMap<User, UserLoginDTO>();
        }
    }
}
