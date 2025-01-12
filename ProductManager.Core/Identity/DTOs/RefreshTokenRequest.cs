// ProductManager.Core/Identity/DTOs/RefreshTokenRequest.cs

namespace ProductManager.Core.Identity.DTOs;

/*
 * RefreshTokenRequest sınıfı, kullanıcıdan gelen yenileme token'ı ile mevcut geçerli token'ın yenilenmesi için gerekli
 * verileri temsil eder.
 * Bu sınıf, kullanıcıdan alınan yenileme taleplerini API'ye iletmek için kullanılır.
 *
 * Özellikler:
 * - Token: Süresi dolmuş olan mevcut JWT token.
 * - RefreshToken: Kullanıcının oturumu yenilemesi için kullanılan geçerli yenileme token'ı.
 */
public class RefreshTokenRequest
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}