// ProductManager.Application/Identity/Interfaces/IAuthService.cs
using ProductManager.Core.Identity.DTOs;

namespace ProductManager.Application.Identity.Interfaces;

/*
 * IAuthService, kimlik doğrulama işlemleri için gereken temel yöntemleri tanımlar.
 * Uygulamanın kimlik doğrulama ve kullanıcı yönetimi işlemlerini kolaylaştırmak için tasarlanmıştır.
 *
 * Yöntemler:
 * - Task<AuthResponse> RegisterAsync(RegisterRequest request): Yeni bir kullanıcı kaydeder ve kimlik doğrulama yanıtı
 *   döner.
 * - Task<AuthResponse> LoginAsync(LoginRequest request): Kullanıcı girişini gerçekleştirir ve kimlik doğrulama yanıtı
 *   döner.
 * - Task<AuthResponse> ValidateTwoFactorAsync(TwoFactorRequest request): İki faktörlü kimlik doğrulama işlemini
 *   gerçekleştirir ve yanıt döner.
 * - Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request): JWT token'ını yeniler ve kimlik doğrulama yanıtı
 *   döner.
 * - Task<bool> ConfirmEmailAsync(string userId, string token): Kullanıcının e-posta adresini doğrular ve başarı
 *   durumunu döner.
 * - Task<bool> ForgotPasswordAsync(string email): Parola sıfırlama talebi oluşturur ve başarı durumunu döner.
 * - Task<bool> ResetPasswordAsync(string email, string token, string newPassword): Parolayı sıfırlar ve başarı durumunu
 *   döner.
 */
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> ValidateTwoFactorAsync(TwoFactorRequest request);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task<bool> ConfirmEmailAsync(string userId, string token);
    Task<bool> ForgotPasswordAsync(string email);
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
}