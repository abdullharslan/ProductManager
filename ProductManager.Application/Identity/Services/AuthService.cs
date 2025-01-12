// ProductManager.Application/Identity/Services/AuthService.cs

using System.Net;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ProductManager.Application.Identity.Interfaces;
using ProductManager.Core.Identity.DTOs;
using ProductManager.Core.Identity.Entities;
using ProductManager.Application.Exceptions;
using ProductManager.Core.Interfaces;

namespace ProductManager.Application.Identity.Services;

/*
 * AuthService, kullanıcı kimlik doğrulama ve yetkilendirme işlemleri için gereken tüm işlevleri sağlar.
 * Bu sınıf, IAuthService arayüzünü uygular ve kullanıcı kayıt, giriş, e-posta onayı, şifre sıfırlama gibi işlemleri
 * yönetir.
 *
 * Metotlar:
 * - Task<AuthResponse> RegisterAsync(RegisterRequest request):
 *   Yeni bir kullanıcıyı kaydeder, e-posta doğrulama bağlantısı oluşturur ve gönderir.
 *
 * - Task<AuthResponse> LoginAsync(LoginRequest request):
 *   Kullanıcıyı doğrular, JWT ve refresh token oluşturur. İki faktörlü kimlik doğrulama gerekiyorsa buna göre yanıt
 *   döner.
 *
 * - Task<AuthResponse> ValidateTwoFactorAsync(TwoFactorRequest request):
 *   Kullanıcının iki faktörlü kimlik doğrulama kodunu doğrular ve JWT ile refresh token üretir.
 *
 * - Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request):
 *   Verilen refresh token'ı kontrol eder ve yeni bir token seti oluşturur.
 *
 * - Task<bool> ConfirmEmailAsync(string userId, string token):
 *   Kullanıcının e-posta adresini doğrular.
 *
 * - Task<bool> ForgotPasswordAsync(string email):
 *   Şifre sıfırlama bağlantısını kullanıcıya e-posta olarak gönderir.
 *
 * - Task<bool> ResetPasswordAsync(string email, string token, string newPassword):
 *   Kullanıcının şifresini verilen token ile sıfırlar.
 *
 * Açıklamalar:
 * - UserManager<ApplicationUser>: ASP.NET Core Identity'nin kullanıcı yönetimi işlemleri için kullanılan sınıf.
 * - ITokenService: JWT ve refresh token işlemleri için kullanılan hizmet.
 * - IEmailService: E-posta işlemleri için kullanılan hizmet (e-posta onayı, şifre sıfırlama vb.).
 * - ILogger<AuthService>: Loglama işlemleri için kullanılan araç.
 *
 * Özelleştirilmiş Hata Yönetimi:
 * - CustomValidationException: Kullanıcı girişine bağlı hatalar için kullanılır.
 * - Hatalar loglanır ve kullanıcı dostu mesajlar döndürülür.
 */
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IEmailService emailService,
        IMapper mapper,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _emailService = emailService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                throw new CustomValidationException("Email is already registered.");
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                throw new CustomValidationException(errors);
            }

            // Generate email confirmation token and build confirmation link
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));
            var confirmationLink = $"https://yourdomain.com/confirm-email?userId={user.Id}&token={encodedToken}";

            // Send confirmation email
            await _emailService.SendEmailConfirmationAsync(user.Email, user.FirstName, confirmationLink);

            return new AuthResponse 
            { 
                Succeeded = true, 
                Message = "Registration successful. Please check your email for confirmation." 
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
            throw;
        }
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !user.IsActive)
            {
                throw new CustomValidationException("Invalid login attempt.");
            }

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                throw new CustomValidationException("Invalid login attempt.");
            }

            if (!user.EmailConfirmed)
            {
                throw new CustomValidationException("Please confirm your email before logging in.");
            }

            if (user.TwoFactorEnabled)
            {
                // Generate and send 2FA code
                var twoFactorToken = _tokenService.GenerateTwoFactorToken();
                user.TwoFactorSecretKey = twoFactorToken;
                await _userManager.UpdateAsync(user);

                // Send 2FA code via email
                await _emailService.SendTwoFactorCodeAsync(user.Email, user.FirstName, twoFactorToken);

                return new AuthResponse
                {
                    Succeeded = true,
                    RequiresTwoFactor = true,
                    Message = "2FA code has been sent to your email."
                };
            }

            var (token, refreshToken) = await _tokenService.GenerateTokensAsync(user);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            user.LastLoginDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return new AuthResponse
            {
                Succeeded = true,
                Token = token,
                RefreshToken = refreshToken,
                Message = "Login successful"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            throw;
        }
    }


    public async Task<AuthResponse> ValidateTwoFactorAsync(TwoFactorRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !user.IsActive)
            {
                throw new CustomValidationException("Invalid request.");
            }

            if (!_tokenService.ValidateTwoFactorToken(user.TwoFactorSecretKey, request.TwoFactorCode))
            {
                throw new CustomValidationException("Invalid 2FA code.");
            }

            var (token, refreshToken) = await _tokenService.GenerateTokensAsync(user);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            user.LastLoginDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return new AuthResponse
            {
                Succeeded = true,
                Token = token,
                RefreshToken = refreshToken,
                Message = "2FA validation successful"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during 2FA validation for email: {Email}", request.Email);
            throw;
        }
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            // Token'dan kullanıcı bilgilerini al
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.Token);
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new CustomValidationException("Invalid token");
            }

            // Kullanıcıyı bul
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                throw new CustomValidationException("User not found or inactive");
            }

            // Refresh token'ı kontrol et
            if (user.RefreshToken != request.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new CustomValidationException("Invalid or expired refresh token");
            }

            // Yeni token'ları üret
            var (newToken, newRefreshToken) = await _tokenService.GenerateTokensAsync(user);

            // Kullanıcı bilgilerini güncelle
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            // Yanıtı döndür
            return new AuthResponse
            {
                Succeeded = true,
                Token = newToken,
                RefreshToken = newRefreshToken,
                Message = "Token refresh successful"
            };
        }
        catch (SecurityTokenException)
        {
            throw new CustomValidationException("Invalid token");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh for token: {Token}", request.Token);
            throw;
        }
    }

    public async Task<bool> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new CustomValidationException("Invalid user.");
        }

        try
        {
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            
            if (result.Succeeded)
            {
                _logger.LogInformation("Email confirmed successfully for user {UserId}", userId);
                return true;
            }
            
            var errors = result.Errors.Select(e => e.Description).ToList();
            throw new CustomValidationException(errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming email for user {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Return true to prevent email enumeration
                return true;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            // https://yourdomain.com yerine gerçek domain adresinizi kullanmalısınız
            var resetLink = $"https://yourdomain.com/reset-password?email={WebUtility.UrlEncode(email)}&token={encodedToken}";

            await _emailService.SendPasswordResetAsync(user.Email, user.FirstName, resetLink);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset request for email: {Email}", email);
            throw;
        }
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            throw new CustomValidationException("Invalid request.");
        }

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            throw new CustomValidationException(errors);
        }

        return true;
    }
}