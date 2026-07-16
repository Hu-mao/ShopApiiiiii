using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Application.DTOs.UserDTOs
{
    public class UserCreateDTO
    {

        [Required]
        [MinLength(5)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(5)]
        public string Password { get; set; } = null!;

    }
}
