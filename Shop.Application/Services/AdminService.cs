using Shop.Application.DTOs.AdminDTOs;
using Shop.Application.Interfaces;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;

namespace Shop.Application.Services;

public class AdminService(
    IAuthRepository _repository,
    IHashHelper _hashHelper,
    IEmailService _emailService) : IAdminService
{
    public async Task CreateStaffAsync(CreateStaffDTO dto)
    {
        var exists = await _repository
            .IsExistEmailAsync(dto.Email);

        if (exists)
            throw new Exception("User with this email already exists");

        var temporaryPassword =
            Guid.NewGuid().ToString("N")[..12];

        var hash = _hashHelper.Hash(temporaryPassword);

        var user = new User
        {
            Email = dto.Email,
            Role = dto.Role,
        };

        await _repository.RegisterUserAsync(
            user,
            hash);

        await _emailService.SendEmailAsync(
            dto.Email,
            "Shop account created",
            $"""
            <h2>Your Shop account was created</h2>

            <p>Your role: <b>{dto.Role}</b></p>

            <p>
                Your temporary password:
                <b>{temporaryPassword}</b>
            </p>

            <p>
                Please login and change your password.
            </p>
            """);
    }
}