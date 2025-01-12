// ProductManager.Core/Identity/DTOs/AuthResponse.cs

namespace ProductManager.Core.Identity.DTOs;

/*
 * AuthResponse sınıfı, kullanıcı kimlik doğrulama (auth) işlemleri sırasında döndürülecek yanıtları temsil eder.
 * Bu sınıf, API'den dönen yanıtların kullanıcıya ne şekilde iletileceğini belirler.
 *
 * Özellikler:
 * - Succeeded: Kimlik doğrulamanın başarıyla tamamlandığını belirtir. Boolean değer döner.
 * - Message: İşlemle ilgili kullanıcıya iletilecek mesaj. Örneğin, başarılı bir kayıt veya giriş mesajı olabilir.
 * - Token: Kullanıcının kimliğini doğrulayan JWT (JSON Web Token) token'ı.
 * - RefreshToken: Token'ın süresi dolduğunda yenilenmesi için kullanılan refresh token.
 * - RequiresTwoFactor: Kullanıcının iki faktörlü doğrulama (2FA) yapıp yapmayacağını belirten bir bayrak. Eğer 2FA
 *   gerekli ise true döner.
 */
public class AuthResponse
{
    public bool Succeeded { get; set; }
    public string Message { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public bool RequiresTwoFactor { get; set; }
}