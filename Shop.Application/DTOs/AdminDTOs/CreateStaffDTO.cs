using Shop.Domain.Enums;

namespace Shop.Application.DTOs.AdminDTOs;

public class CreateStaffDTO
{
    public string Email { get; set; } = string.Empty;

    public UserRole Role { get; set; }
}