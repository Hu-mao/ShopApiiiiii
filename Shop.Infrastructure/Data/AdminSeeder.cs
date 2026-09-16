using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces;
using Shop.Domain.Enums;
using Shop.Domain.Models;

namespace Shop.Infrastructure.Data;

public static class AdminSeeder
{
    public static async Task SeedAsync(
        ShopDbContext context,
        IHashHelper hashHelper)
    {
        var adminExists = await context.Users
            .AnyAsync(x => x.Role == UserRole.Admin);

        if (adminExists)
            return;

        var admin = new User
        {
            Email = "admin@shop.com",
            Role = UserRole.Admin
        };

        var hash = hashHelper.Hash("Admin123!");

        //admin.Password = hash;

        context.Users.Add(admin);

        await context.SaveChangesAsync();
    }
}