using Shop.Application.DTOs.AdminDTOs;

namespace Shop.Application.Interfaces.Services;

public interface IAdminService
{
    Task CreateStaffAsync(CreateStaffDTO dto);
}