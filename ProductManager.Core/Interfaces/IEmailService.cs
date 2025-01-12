// ProductManager.Core/Interfaces/IEmailService.cs

namespace ProductManager.Core.Interfaces;

/*
 * IEmailService arayüzü, uygulama içindeki e-posta gönderme işlemlerini tanımlar.
 * Bu arayüz, e-posta ile ilgili çeşitli işlevleri içeren metodlar sunar.
 * Implementasyonu gerçekleştiren sınıf, belirtilen işlevleri yerine getirecektir.
 *
 * Yöntemler:
 * - SendEmailAsync: Belirtilen e-posta adresine, konu ve içerik ile bir e-posta gönderir.
 * - SendEmailConfirmationAsync: E-posta doğrulaması için bir bağlantı ile e-posta gönderir.
 * - SendPasswordResetAsync: Şifre sıfırlama bağlantısı içeren bir e-posta gönderir.
 * - SendTwoFactorCodeAsync: İki faktörlü doğrulama kodu içeren bir e-posta gönderir.
 */
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = false);
    Task SendEmailConfirmationAsync(string email, string firstName, string confirmationLink);
    Task SendPasswordResetAsync(string email, string firstName, string resetLink);
    Task SendTwoFactorCodeAsync(string email, string firstName, string code);
}