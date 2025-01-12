// ProductManager.Application/Identity/Services/TokenService.cs

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProductManager.Application.Identity.Interfaces;
using ProductManager.Core.Identity.Entities;
using OtpNet;

namespace ProductManager.Application.Identity.Services;

/*
 * TokenService, JWT, Refresh Token ve İki Faktörlü Kimlik Doğrulama (TOTP) işlemlerini yönetmek için tasarlanmış bir servistir.
 * ITokenService arayüzünü uygular.
 *
 * Metotlar:
 *
 * - Task<(string Token, string RefreshToken)> GenerateTokensAsync(ApplicationUser user):
 *   Kullanıcı bilgilerine göre bir JWT ve refresh token üretir.
 *
 * - Task<bool> ValidateTokenAsync(string token):
 *   Verilen JWT'yi doğrular. Geçerliyse `true`, aksi takdirde `false` döner.
 *
 * - ClaimsPrincipal GetPrincipalFromExpiredToken(string token):
 *   Süresi dolmuş bir token'dan kullanıcı bilgilerini alır. Geçersiz token durumunda `SecurityTokenException` fırlatır.
 *
 * - string GenerateTwoFactorToken():
 *   İki faktörlü kimlik doğrulama için rastgele bir TOTP secret key üretir.
 *
 * - bool ValidateTwoFactorToken(string secretKey, string code):
 *   Kullanıcının sağladığı TOTP kodunu verilen secret key ile doğrular.
 *
 * Özel Metotlar:
 * - string GenerateRefreshToken():
 *   Rastgele bir refresh token üretir.
 *
 * Açıklamalar:
 * - JWT Üretimi:
 *   - `SymmetricSecurityKey`: HMACSHA256 algoritması için kullanılan simetrik anahtar.
 *   - `SigningCredentials`: Token'in imzalanması için gerekli olan güvenlik bilgileri.
 *   - Süre: Varsayılan olarak 1 saat.
 *
 * - Refresh Token:
 *   - Rastgele 32 baytlık bir dizi üretilir ve Base64'e çevrilir.
 *   - Güvenli bir rastgele sayı üreteci (`RandomNumberGenerator`) kullanılır.
 *
 * - TOTP:
 *   - `OtpNet` kütüphanesi kullanılır.
 *   - `Totp` sınıfı, iki faktörlü kimlik doğrulama kodlarının doğrulanmasını sağlar.
 *   - Kullanıcı kodu ve sunucu tarafındaki TOTP secret key uyumluysa doğrulama başarılı olur.
 *
 * Hata Yönetimi:
 * - JWT doğrulaması sırasında bir hata oluşursa `false` döner.
 * - Geçersiz veya süresi dolmuş token durumunda ilgili özel durumlar fırlatılır.
 */
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<(string Token, string RefreshToken)> GenerateTokensAsync(ApplicationUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_SECRET_KEY"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddHours(1);

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT_ISSUER"],
            audience: _configuration["JWT_AUDIENCE"],
            claims: claims,
            expires: expiry,
            signingCredentials: credentials
        );

        var refreshToken = GenerateRefreshToken();

        return (new JwtSecurityTokenHandler().WriteToken(token), refreshToken);
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JWT_SECRET_KEY"]);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _configuration["JWT_ISSUER"],
                ValidAudience = _configuration["JWT_AUDIENCE"],
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWT_SECRET_KEY"])),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = _configuration["JWT_ISSUER"],
            ValidAudience = _configuration["JWT_AUDIENCE"],
            ValidateLifetime = false // Önemli: Süresi dolmuş token'ı validate edebilmek için false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || 
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, 
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    public string GenerateTwoFactorToken()
    {
        // Generate a random secret key for TOTP
        var secretKey = KeyGeneration.GenerateRandomKey(20);
        return Base32Encoding.ToString(secretKey);
    }

    public bool ValidateTwoFactorToken(string secretKey, string code)
    {
        if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(code))
            return false;

        try
        {
            var base32Bytes = Base32Encoding.ToBytes(secretKey);
            var totp = new Totp(base32Bytes);
            return totp.VerifyTotp(code, out long timeStepMatched);
        }
        catch
        {
            return false;
        }
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}