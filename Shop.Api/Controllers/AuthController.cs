
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shop.Application.DTOs.UserDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Infrastructure.Configuration;
using System.Security.Claims;

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

     [HttpGet("login-google")]
        public IActionResult LoginGoogle()

{

    var properties = new AuthenticationProperties
    {

        RedirectUri = Url.Action(nameof(ExternalResponse))

    };

    return Challenge(properties, GoogleDefaults.AuthenticationScheme);

}

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

        [HttpGet("external-response")]
        public async Task<IActionResult> ExternalResponse()

{

    var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);


    if (!result.Succeeded)

        return BadRequest("Помилка зовнішньої аутентифікації.");


        var claims = result.Principal.Identities.FirstOrDefault()?.Claims;


        var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        var providerId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        return Ok(new { Name = name, Email = email, ProviderId = providerId });

        
   
}

        [HttpPost("logout")]

        [Authorize]

        public async Task<IActionResult> Logout()

        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok("Вихід успішний.");

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
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(
    ForgotPasswordDTO dto)
        {
            await _authService.ForgotPasswordAsync(dto);

            return Ok(new
            {
                message =
                    "If the email exists, a password reset link has been sent."
            });
        }
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(
    ResetPasswordDTO dto)
        {
            await _authService.ResetPasswordAsync(dto);

            return Ok(new
            {
                message = "Password changed successfully"
            });
        }
    }
}

