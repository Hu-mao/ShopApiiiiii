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
        IConfiguration _configuration
    ) : IAuthService
    {
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
    }
}