// ProductManager.Application/Identity/Validators/LoginRequestValidator.cs
using FluentValidation;
using ProductManager.Core.Identity.DTOs;

namespace ProductManager.Application.Identity.Validators;

/*
 * LoginRequestValidator, LoginRequest nesnesinin doğruluğunu kontrol etmek için kullanılan bir doğrulama sınıfıdır.
 * FluentValidation kütüphanesini kullanarak, login işlemi için gerekli olan e-posta ve şifre bilgilerini doğrular.
 *
 * Kurallar:
 *
 * - Email:
 *   - Boş olmamalıdır (`NotEmpty`).
 *   - Geçerli bir e-posta adresi formatında olmalıdır (`EmailAddress`).
 *   - Hatalı bir e-posta girildiğinde, kullanıcıya "A valid email address is required." mesajı gösterilecektir.
 *
 * - Password:
 *   - Boş olmamalıdır (`NotEmpty`).
 *   - Hatalı bir şifre girildiğinde, kullanıcıya "Password is required." mesajı gösterilecektir.
 *
 * Kullanım:
 * - Bu sınıf, genellikle login istekleri için doğrulama yapılırken kullanılır.
 * - Kullanıcıdan alınan login bilgileri, bu doğrulama kuralları ile kontrol edilir.
 */
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}