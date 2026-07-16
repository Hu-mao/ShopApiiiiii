using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Domain.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime ExpireDate { get; set; }

        public bool IsRevoked { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; } = null!;
    }
}
