using Shop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Shop.Domain.Models;

[Table("users")] 
public class User : BaseEntity {

    [Column("id")] 
    public Guid Id { get; set; } = Guid.NewGuid(); 
    
    
    [Required][EmailAddress][Column("email")] 
    public string Email { get; set; } = string.Empty; 
    
    
    [Required][Column("password_hash")] 
    public string PasswordHash { get; set; } = string.Empty; 
    [Column("role")] 
    public UserRole Role { get; set; } = UserRole.User; 
    
    
    [Column("is_active")] 
    public bool IsActive { get; set; } = true; 
}
