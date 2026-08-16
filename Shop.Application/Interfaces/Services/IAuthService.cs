using Shop.Application.DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO?> RegisterAsync(UserCreateDTO dto);
        Task<AuthResponseDTO?> RefreshAsync(string refreshToken);
        Task<UserReadDTO?> CreateAdminAsync(AdminCreateDTO dto);
    }
}
