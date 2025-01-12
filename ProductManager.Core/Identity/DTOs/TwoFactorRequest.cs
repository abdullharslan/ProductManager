// ProductManager.Core/Identity/DTOs/TwoFactorRequest.cs

namespace ProductManager.Core.Identity.DTOs;

/*
 * TwoFactorRequest sınıfı, kullanıcıdan alınan iki faktörlü doğrulama bilgilerini temsil eder.
 * Kullanıcıdan alınan e-posta adresi ve 2FA kodu, iki faktörlü kimlik doğrulaması için kullanılır.
 *
 * Özellikler:
 * - Email: Kullanıcının e-posta adresi.
 * - TwoFactorCode: Kullanıcıya gönderilen ve doğrulama için kullanılan 2FA kodu.
 */
public class TwoFactorRequest
{
    public string Email { get; set; }
    public string TwoFactorCode { get; set; }
}