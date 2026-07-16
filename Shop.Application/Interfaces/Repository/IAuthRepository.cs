using Shop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Interfaces.Repository
{
    public interface IAuthRepository
    {
        Task<User?> RegisterUserAsync(User user, string hash);
        Task<bool> IsExistEmailAsync(string email);
    }
}
