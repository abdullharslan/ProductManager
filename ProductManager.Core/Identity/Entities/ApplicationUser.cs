// ProductManager.Core/Identity/Entities/ApplicationUser.cs

using Microsoft.AspNetCore.Identity;

namespace ProductManager.Core.Identity.Entities;

/*
 * ApplicationUser sınıfı, IdentityUser sınıfından türetilmiştir ve uygulamanın kullanıcı bilgilerini temsil eder.
 * Bu sınıf, kullanıcının kimlik doğrulama bilgileri ile birlikte kullanıcıya ait ek özellikleri içerir.
 * Özellikle, kullanıcının adı, oluşturulma tarihi, son giriş tarihi ve güvenlik özellikleri bu sınıfta yer alır.
 *
 * Özellikler:
 * - FirstName: Kullanıcının adı.
 * - LastName: Kullanıcının soyadı.
 * - CreatedDate: Kullanıcının oluşturulma tarihi.
 * - LastLoginDate: Kullanıcının son giriş tarihi.
 * - IsActive: Kullanıcının aktif olup olmadığını belirtir.
 * - RefreshToken: Kullanıcıya ait yenileme token'ı.
 * - RefreshTokenExpiryTime: Yenileme token'ının geçerlilik süresi.
 * - TwoFactorEnabled: İki faktörlü doğrulamanın etkin olup olmadığını belirtir.
 * - TwoFactorSecretKey: İki faktörlü doğrulama için kullanılan gizli anahtar.
 */
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public bool IsActive { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public string? TwoFactorSecretKey { get; set; }
}