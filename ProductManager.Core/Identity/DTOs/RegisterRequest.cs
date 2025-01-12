// ProductManager.Core/Identity/DTOs/RegisterRequest.cs

namespace ProductManager.Core.Identity.DTOs;

/*
 * RegisterRequest sınıfı, kullanıcı kaydını başlatmak için gerekli verileri temsil eder.
 * Kullanıcıdan alınan kayıt bilgileri (isim, e-posta, şifre vb.) bu sınıf aracılığıyla API'ye iletilir.
 *
 * Özellikler:
 * - FirstName: Kullanıcının adı.
 * - LastName: Kullanıcının soyadı.
 * - Email: Kullanıcının e-posta adresi.
 * - Password: Kullanıcı tarafından belirlenen şifre.
 * - ConfirmPassword: Kullanıcı tarafından girilen şifrenin teyidi.
 */
public class RegisterRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}