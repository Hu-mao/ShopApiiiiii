using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shop.Application.DTOs.UserDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Infrastructure.Configuration;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(
    IAuthService _authService,
    IOptions<JwtSettings> _jwtOptions) : ControllerBase
{
    private readonly JwtSettings _jwtSettings = _jwtOptions.Value;

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(
        [FromBody] UserCreateDTO dto)
    {
        var result = await _authService.RegisterAsync(dto);

        if (result == null)
            return BadRequest("Користувач за таким email вже існує");

        SetRefreshTokenCookie(result.RefreshToken!);

        return Ok(new
        {
            user = result.User,
            accessToken = result.Token
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] UserLoginDTO dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (result == null)
            return Unauthorized("Невірний email або пароль");

        SetRefreshTokenCookie(result.RefreshToken!);

        return Ok(new
        {
            user = result.User,
            accessToken = result.Token
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized("Refresh token not found.");

        var result = await _authService.RefreshAsync(refreshToken);

        if (result == null)
        {
            Response.Cookies.Delete("refreshToken");

            return Unauthorized("Invalid or expired refresh token.");
        }

        SetRefreshTokenCookie(result.RefreshToken!);

        return Ok(new
        {
            accessToken = result.Token
        });
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append(
            "refreshToken",
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(
                    _jwtSettings.ExpiresRefreshTokenDay)
            });
    }
}