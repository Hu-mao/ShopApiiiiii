using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Repository;

public interface IPasswordResetTokenRepository
{
    Task AddAsync(PasswordResetToken token);

    Task<PasswordResetToken?> GetByTokenAsync(string token);

    Task MarkAsUsedAsync(PasswordResetToken token);
}