using Shop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.DTOs.UserDTOs
{
    public class UserReadDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;

    }
}
