using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Repository;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);

    Task<RefreshToken?> GetTokenAsync(string token);

    Task UpdateAsync(RefreshToken token);
}