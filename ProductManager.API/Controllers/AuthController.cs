// ProductManager.API/Controllers/AuthController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManager.API.Models;
using ProductManager.Application.Identity.Interfaces;
using ProductManager.Core.Identity.DTOs;

namespace ProductManager.API.Controllers;

/*
 * AuthController, kimlik doğrulama işlemleri için HTTP endpoint'lerini sağlar.
 * RESTful prensiplerine uygun olarak tasarlanmıştır.
 *
 * Endpoint'ler:
 * - POST /api/auth/register: Yeni kullanıcı kaydı yapar
 * - POST /api/auth/login: Kullanıcı girişi yapar
 * - POST /api/auth/two-factor: İki faktörlü kimlik doğrulama yapar
 * - POST /api/auth/refresh-token: JWT yenileme işlemi yapar
 * - GET /api/auth/confirm-email: E-posta doğrulama işlemini tamamlar
 * - POST /api/auth/forgot-password: Parola sıfırlama talebi gönderir
 * - POST /api/auth/reset-password: Parola sıfırlama işlemini gerçekleştirir
 */
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);
        return Ok(response);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    [HttpPost("two-factor")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidateTwoFactor([FromBody] TwoFactorRequest request)
    {
        var response = await _authService.ValidateTwoFactorAsync(request);
        return Ok(response);
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var response = await _authService.RefreshTokenAsync(request);
        return Ok(response);
    }

    [HttpGet("confirm-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
    {
        var result = await _authService.ConfirmEmailAsync(userId, token);
        if (result)
            return Ok("Email confirmed successfully.");
        return BadRequest("Email confirmation failed.");
    }

    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] string email)
    {
        await _authService.ForgotPasswordAsync(email);
        return Ok("If your email is registered with us, you will receive a password reset link.");
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromQuery] string email, [FromQuery] string token, [FromBody] string newPassword)
    {
        var result = await _authService.ResetPasswordAsync(email, token, newPassword);
        if (result)
            return Ok("Password has been reset successfully.");
        return BadRequest("Password reset failed.");
    }
}