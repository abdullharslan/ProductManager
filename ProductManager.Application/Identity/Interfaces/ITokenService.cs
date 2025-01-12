// ProductManager.Application/Identity/Interfaces/ITokenService.cs

using System.Security.Claims;
using ProductManager.Core.Identity.Entities;

namespace ProductManager.Application.Identity.Interfaces;

/*
 * ITokenService, uygulamanın token üretimi, doğrulaması ve yönetimi için gerekli yöntemleri tanımlar.
 * Bu hizmet, JWT ve iki faktörlü doğrulama token'larının işlenmesini kolaylaştırır.
 *
 * Yöntemler:
 * - Task<(string Token, string RefreshToken)> GenerateTokensAsync(ApplicationUser user):
 *   Kullanıcı için JWT ve yenileme token'ı oluşturur.
 *
 * - Task<bool> ValidateTokenAsync(string token):
 *   Verilen token'ın geçerliliğini kontrol eder ve geçerlilik durumunu döner.
 *
 * - string GenerateTwoFactorToken():
 *   İki faktörlü kimlik doğrulama için bir token oluşturur.
 *
 * - bool ValidateTwoFactorToken(string secretKey, string code):
 *   İki faktörlü kimlik doğrulama token'ını doğrular ve geçerlilik durumunu döner.
 *
 * - ClaimsPrincipal GetPrincipalFromExpiredToken(string token):
 *   Süresi dolmuş bir token'dan ClaimsPrincipal çıkarır ve döner.
 */
public interface ITokenService
{
    Task<(string Token, string RefreshToken)> GenerateTokensAsync(ApplicationUser user);
    Task<bool> ValidateTokenAsync(string token);
    string GenerateTwoFactorToken();
    bool ValidateTwoFactorToken(string secretKey, string code);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}