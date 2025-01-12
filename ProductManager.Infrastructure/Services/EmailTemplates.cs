// ProductManager.Infrastructure/Services/EmailTemplates.cs

namespace ProductManager.Infrastructure.Services;

/*
 * EmailTemplates sınıfı, kullanıcıya gönderilecek e-posta içeriklerini oluşturmak için statik metodlar içerir.
 * Bu sınıf, e-posta içeriğinin HTML formatında dinamik olarak hazırlanmasına olanak tanır.
 *
 * Sınıfın Metodları:
 * - GetEmailConfirmationTemplate: Kullanıcıya e-posta onayı göndermek için HTML şablonunu döndürür. İçeriğinde kullanıcı
 *   adı ve onay linki yer alır.
 * - GetPasswordResetTemplate: Şifre sıfırlama işlemi için bir e-posta şablonu döndürür. Kullanıcı adı ve sıfırlama
 *   linki içerir.
 * - GetTwoFactorCodeTemplate: İki faktörlü doğrulama kodu göndermek için şablon döndürür. Kod, kullanıcı adı ile
 *   birlikte HTML formatında gösterilir.
 *
 * Her metod, ilgili işlemi yapmak için e-posta içeriğini özelleştirir. E-posta, HTML formatında döndürülür ve stil ile
 * özelleştirilmiş içerik barındırır.
 */
public static class EmailTemplates
{
    public static string GetEmailConfirmationTemplate(string firstName, string confirmationLink)
    {
        return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Welcome to Product Manager, {firstName}!</h2>
                <p>Please confirm your email address by clicking the link below:</p>
                <p>
                    <a href='{confirmationLink}' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                        Confirm Email
                    </a>
                </p>
                <p>If the button doesn't work, you can also copy and paste this link in your browser:</p>
                <p>{confirmationLink}</p>
                <p>This link will expire in 24 hours.</p>
            </body>
            </html>";
    }

    public static string GetPasswordResetTemplate(string firstName, string resetLink)
    {
        return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Password Reset Request</h2>
                <p>Hi {firstName},</p>
                <p>We received a request to reset your password. Click the link below to choose a new password:</p>
                <p>
                    <a href='{resetLink}' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                        Reset Password
                    </a>
                </p>
                <p>If you didn't request this, you can safely ignore this email.</p>
                <p>The reset link will expire in 1 hour.</p>
            </body>
            </html>";
    }

    public static string GetTwoFactorCodeTemplate(string firstName, string code)
    {
        return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Your Two-Factor Authentication Code</h2>
                <p>Hi {firstName},</p>
                <p>Your verification code is:</p>
                <h1 style='font-size: 32px; letter-spacing: 5px; background-color: #f5f5f5; padding: 10px; text-align: center;'>
                    {code}
                </h1>
                <p>This code will expire in 10 minutes.</p>
                <p>If you didn't request this code, please secure your account by changing your password.</p>
            </body>
            </html>";
    }
}