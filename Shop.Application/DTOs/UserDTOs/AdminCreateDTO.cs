using System.ComponentModel.DataAnnotations;

namespace Shop.Application.DTOs.UserDTOs;

public class AdminCreateDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(5)]
    public string Password { get; set; } = string.Empty;
}