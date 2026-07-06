using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Domain.Models;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string HashPassword { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
