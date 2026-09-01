
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Shop.Application.DTOs.UserDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Infrastructure.Configuration;

namespace Shop.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController(
        IAuthService _authService,
        IOptions<JwtSettings> _jwtSettings) : ControllerBase
    {
        private const string RefreshTokenCookieName =
            "refreshToken";


        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(
            [FromBody] UserCreateDTO dto)
        {
            var result =
                await _authService.RegisterAsync(dto);

            if (result == null)
                return BadRequest(
                    "Користувач за таким email вже існує");

            SetRefreshTokenCookie(
                result.RefreshToken!);

            return Ok(new
            {
                user = result.User,
                accessToken = result.Token,
                refreshToken = result.RefreshToken
            });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] UserLoginDTO dto)
        {
            var result =
                await _authService.LoginAsync(dto);

            if (result == null)
                return Unauthorized(
                    "Невірний email або пароль");

            SetRefreshTokenCookie(
                result.RefreshToken!);

            return Ok(new
            {
                user = result.User,
                accessToken = result.Token,
                refreshToken = result.RefreshToken
            });
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken =
                Request.Cookies[RefreshTokenCookieName];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(
                    "Refresh token not found.");


            var result =
                await _authService.RefreshAsync(
                    refreshToken);

            if (result == null)
                return Unauthorized(
                    "Invalid or expired refresh token.");


            SetRefreshTokenCookie(
                result.RefreshToken!);

            return Ok(new
            {
                accessToken = result.Token,
                refreshToken = result.RefreshToken
            });
        }


        private void SetRefreshTokenCookie(
            string refreshToken)
        {
            Response.Cookies.Append(
                RefreshTokenCookieName,
                refreshToken,
                new CookieOptions
                {
                    HttpOnly = true,

                    // Якщо API працює через HTTPS:
                    // Secure = true
                    Secure = false,

                    SameSite = SameSiteMode.Strict,

                    Expires =
                        DateTimeOffset.UtcNow.AddDays(
                            _jwtSettings.Value
                                .ExpiresRefreshTokenDay)
                });
        }
    }
}

