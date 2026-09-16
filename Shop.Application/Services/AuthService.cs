using AutoMapper;
using Microsoft.Extensions.Configuration;

using Shop.Application.DTOs.UserDTOs;
using Shop.Application.Interfaces;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;

using Shop.Domain.Enums;
using Shop.Domain.Models;

namespace Shop.Application.Services
{

    public class AuthService(
        IMapper _mapper,
        IAuthRepository _repository,
        IRefreshTokenRepository _refreshTokenRepository,
        IHashHelper _hashHelper,
        IJWTService _jwtService,
        IConfiguration _configuration,
        IPasswordResetTokenRepository passwordResetTokenRepository,
IEmailService emailService
    ) : IAuthService
    {
        private readonly IPasswordResetTokenRepository
    _passwordResetTokenRepository = passwordResetTokenRepository;

        private readonly IEmailService
            _emailService = emailService;
        public async Task<AuthResponseDTO?> RegisterAsync(
            UserCreateDTO dto)
        {
            var isExist =
                await _repository.IsExistEmailAsync(dto.Email);

            if (isExist)
                return null;

            var hash =
                _hashHelper.Hash(dto.Password);

            var user =
                _mapper.Map<User>(dto);

            var registerUser =
                await _repository.RegisterUserAsync(user, hash);

            if (registerUser == null)
                return null;

            var accessToken =
                _jwtService.GenerateAccessToken(
                    _mapper.Map<UserLoginDTO>(registerUser),
                    registerUser.Role.ToString());

            var refreshToken =
                CreateRefreshToken(registerUser.Id);

            await _refreshTokenRepository.AddAsync(refreshToken);

            return new AuthResponseDTO
            {
                User = _mapper.Map<UserReadDTO>(registerUser),
                Token = accessToken,
                RefreshToken = refreshToken.Token
            };
        }


        public async Task<AuthResponseDTO?> LoginAsync(
            UserLoginDTO dto)
        {
            var hash =
                _hashHelper.Hash(dto.Password);

            var user =
                await _repository.LoginAsync(
                    dto.Email,
                    hash);

            if (user == null)
                return null;

            var accessToken =
                _jwtService.GenerateAccessToken(
                    dto,
                    user.Role.ToString());

            var refreshToken =
                CreateRefreshToken(user.Id);

            await _refreshTokenRepository.AddAsync(refreshToken);

            return new AuthResponseDTO
            {
                User = _mapper.Map<UserReadDTO>(user),
                Token = accessToken,
                RefreshToken = refreshToken.Token
            };
        }


        public async Task<AuthResponseDTO?> RefreshAsync(
            string refreshToken)
        {
            var token =
                await _refreshTokenRepository
                    .GetTokenAsync(refreshToken);

            if (token == null)
                return null;

            if (token.IsRevoked)
                return null;

            if (token.ExpireDate <= DateTime.UtcNow)
                return null;

            if (!token.User.IsActive)
                return null;

            var accessToken =
                _jwtService.GenerateAccessToken(
                    _mapper.Map<UserLoginDTO>(token.User),
                    token.User.Role.ToString());

            return new AuthResponseDTO
            {
                User = _mapper.Map<UserReadDTO>(token.User),
                Token = accessToken,
                RefreshToken = token.Token
            };
        }


        private RefreshToken CreateRefreshToken(Guid userId)
        {
            var expiresDays = int.Parse(
     _configuration["JwtSettings:ExpiresRefreshTokenDay"]!
 );

            return new RefreshToken
            {
                Token = Convert.ToBase64String(
                    System.Security.Cryptography
                        .RandomNumberGenerator
                        .GetBytes(64)),

                UserId = userId,

                ExpireDate =
                    DateTime.UtcNow.AddDays(expiresDays),

                IsRevoked = false
            };
        }


        public async Task<UserReadDTO?> CreateAdminAsync(
            AdminCreateDTO dto)
        {
            var isExist =
                await _repository.IsExistEmailAsync(dto.Email);

            if (isExist)
                return null;

            var hash =
                _hashHelper.Hash(dto.Password);

            var user = new User
            {
                Email = dto.Email,
                Role = UserRole.Admin,
                IsActive = true
            };

            var admin =
                await _repository.CreateAdminAsync(
                    user,
                    hash);

            if (admin == null)
                return null;

            return _mapper.Map<UserReadDTO>(admin);
        }
        public async Task ForgotPasswordAsync(
    ForgotPasswordDTO dto)
        {
            var user = await _repository
                .GetUserByEmailAsync(dto.Email);

            if (user == null)
                return;

            var token = Guid.NewGuid().ToString("N");

            var resetToken = new PasswordResetToken
            {
                Token = token,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsUsed = false
            };

            await _passwordResetTokenRepository
                .AddAsync(resetToken);

            var frontendUrl =
                "http://localhost:5173/reset-password";

            var link =
                $"{frontendUrl}?email={Uri.EscapeDataString(dto.Email)}&token={token}";

            await _emailService.SendEmailAsync(
                dto.Email,
                "Reset your password",
                $"""
        <h2>Password reset</h2>

        <p>
            You requested a password reset.
        </p>

        <p>
            <a href="{link}">
                Reset password
            </a>
        </p>

        <p>
            This link expires in 1 hour.
        </p>
        """);
        }
        public async Task ResetPasswordAsync(
    ResetPasswordDTO dto)
        {
            var resetToken =
                await _passwordResetTokenRepository
                    .GetByTokenAsync(dto.Token);

            if (resetToken == null)
                throw new Exception("Invalid reset token");

            if (resetToken.IsUsed)
                throw new Exception("Reset token already used");

            if (resetToken.ExpiresAt < DateTime.UtcNow)
                throw new Exception("Reset token expired");

            if (!string.Equals(
                    resetToken.User.Email,
                    dto.Email,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Invalid reset token");
            }

            var hash = _hashHelper.Hash(dto.NewPassword);

            resetToken.User.PasswordHash = hash;

            await _passwordResetTokenRepository
                .MarkAsUsedAsync(resetToken);
        }
       

    }
}