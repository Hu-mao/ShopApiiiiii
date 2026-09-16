using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories;

public class PasswordResetTokenRepository(
    ShopDbContext _context) : IPasswordResetTokenRepository
{
    public async Task AddAsync(
        PasswordResetToken token)
    {
        await _context.PasswordResetTokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }

    public async Task<PasswordResetToken?> GetByTokenAsync(
        string token)
    {
        return await _context.PasswordResetTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == token);
    }

    public async Task MarkAsUsedAsync(
        PasswordResetToken token)
    {
        token.IsUsed = true;

        await _context.SaveChangesAsync();
    }
}