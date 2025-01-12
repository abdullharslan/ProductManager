// ProductManager.Application/Identity/Validators/RefreshTokenRequestValidator.cs
using FluentValidation;
using ProductManager.Core.Identity.DTOs;

namespace ProductManager.Application.Identity.Validators;

/*
 * RefreshTokenRequestValidator, RefreshTokenRequest nesnesinin doğruluğunu kontrol etmek için kullanılan bir doğrulama sınıfıdır.
 * FluentValidation kütüphanesini kullanarak, refresh token işlemi için gerekli olan token ve refresh token bilgilerini doğrular.
 *
 * Kurallar:
 *
 * - Token:
 *   - Boş olmamalıdır (`NotEmpty`).
 *   - Hatalı bir token girildiğinde, kullanıcıya "Token is required." mesajı gösterilecektir.
 *
 * - RefreshToken:
 *   - Boş olmamalıdır (`NotEmpty`).
 *   - Hatalı bir refresh token girildiğinde, kullanıcıya "Refresh token is required." mesajı gösterilecektir.
 *
 * Kullanım:
 * - Bu sınıf, genellikle refresh token işlemleri için doğrulama yapılırken kullanılır.
 * - Kullanıcıdan alınan refresh token bilgileri, bu doğrulama kuralları ile kontrol edilir.
 */
public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required.");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}