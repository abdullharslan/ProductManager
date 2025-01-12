// ProductManager.Infrastructure/Services/SmtpEmailService.cs

using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductManager.Core.Interfaces;

namespace ProductManager.Infrastructure.Services;

/*
 * SmtpEmailService, IEmailService arayüzünü implement eden bir sınıftır. Bu sınıf, SMTP protokolü kullanarak e-posta
 * göndermek için gerekli metodları içerir.
 * E-posta gönderme işlemleri için gerekli konfigürasyon değerleri yapılandırma dosyasından alınır ve sınıfın
 * başlangıcında ayarlanır.
 *
 * Özellikler:
 * - _smtpServer, _smtpPort, _smtpUsername, _smtpPassword, _fromEmail, _fromName: SMTP sunucusu ve kimlik doğrulama için
 *   gerekli konfigürasyon değerleri.
 *
 * Metodlar:
 * - SendEmailAsync: Genel bir e-posta gönderme metodudur. Alıcı, konu, içerik ve HTML formatı gibi parametreler ile
 *   e-posta gönderir.
 * - SendEmailConfirmationAsync: Kullanıcıya e-posta onay linki göndermek için kullanılır. Onay linki içeren bir e-posta
 *   şablonu kullanır.
 * - SendPasswordResetAsync: Şifre sıfırlama işlemi için e-posta gönderir. Şifre sıfırlama linki içeren bir şablon
 *   kullanılır.
 * - SendTwoFactorCodeAsync: Kullanıcıya iki faktörlü doğrulama kodu gönderir. Doğrulama kodu içeren bir e-posta şablonu
 *   kullanılır.
 *
 * Sınıf, e-posta gönderim işlemleri sırasında oluşan hataları loglar ve e-posta gönderme başarısız olduğunda istisna
 * fırlatır. E-posta içerikleri, EmailTemplates sınıfındaki statik metodlar aracılığıyla dinamik olarak oluşturulur.
 */
public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailService> _logger;
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _smtpServer = _configuration["SMTP_SERVER"];
        _smtpPort = int.Parse(_configuration["SMTP_PORT"]);
        _smtpUsername = _configuration["SMTP_USERNAME"];
        _smtpPassword = _configuration["SMTP_PASSWORD"];
        _fromEmail = _configuration["FROM_EMAIL"];
        _fromName = _configuration["FROM_NAME"];
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
    {
        try
        {
            var message = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            message.To.Add(new MailAddress(to));

            using var client = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                EnableSsl = true,
                Timeout = 20000 // 20 saniye timeout
            };

            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent successfully to {Email}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {Email}", to);
            throw;
        }
    }

    public async Task SendEmailConfirmationAsync(string email, string firstName, string confirmationLink)
    {
        var body = EmailTemplates.GetEmailConfirmationTemplate(firstName, confirmationLink);
        await SendEmailAsync(email, "Confirm Your Email", body, true);
    }

    public async Task SendPasswordResetAsync(string email, string firstName, string resetLink)
    {
        var body = EmailTemplates.GetPasswordResetTemplate(firstName, resetLink);
        await SendEmailAsync(email, "Password Reset Request", body, true);
    }

    public async Task SendTwoFactorCodeAsync(string email, string firstName, string code)
    {
        var body = EmailTemplates.GetTwoFactorCodeTemplate(firstName, code);
        await SendEmailAsync(email, "Your Two-Factor Authentication Code", body, true);
    }
}