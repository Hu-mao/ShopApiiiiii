using Shop.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Infrastructure.Helpers
{
    public class HashHelper : IHashHelper
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }

        public bool IsValidPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
        }
    }
}
