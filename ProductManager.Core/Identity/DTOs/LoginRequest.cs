// ProductManager.Core/Identity/DTOs/LoginRequest.cs

namespace ProductManager.Core.Identity.DTOs;

/*
 * LoginRequest sınıfı, kullanıcı giriş işlemi sırasında gönderilen verileri temsil eder.
 * Bu sınıf, kullanıcıdan alınan giriş bilgilerini API'ye iletmek için kullanılır.
 *
 * Özellikler:
 * - Email: Kullanıcının giriş yaparken kullanacağı e-posta adresi.
 * - Password: Kullanıcının giriş şifresi.
 * - Remember: Kullanıcının oturum açtıktan sonra hatırlanıp hatırlanmayacağını belirten bir bayrak. True ise kullanıcı
 *   oturumu açıldığında uzun süreli oturum açma tercih edilir.
 */
public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public bool Remember { get; set; }
}