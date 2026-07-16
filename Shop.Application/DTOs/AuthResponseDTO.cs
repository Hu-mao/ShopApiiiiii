using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.DTOs.UserDTOs;

public class AuthResponseDTO
{
    public UserReadDTO? User { get; set; }

    public string? Token { get; set; }

    public string? RefreshToken { get; set; }
}
