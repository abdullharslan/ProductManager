// ProductManager.Application/Identity/Validators/TwoFactorRequestValidator.cs
using FluentValidation;
using ProductManager.Core.Identity.DTOs;

namespace ProductManager.Application.Identity.Validators;

/*
 * TwoFactorRequestValidator, iki faktörlü kimlik doğrulama (2FA) talebini doğrulamak için kullanılan bir sınıftır.
 * FluentValidation kütüphanesini kullanarak, 2FA kodu ve e-posta adresi gibi bilgilerin doğruluğunu kontrol eder. Bu
 * doğrulamalar, 2FA işleminin güvenli ve doğru bir şekilde gerçekleşmesini sağlar.
 *
 * Kurallar:
 *
 * - Email:
 *   - Boş olmamalıdır (`NotEmpty`).
 *   - Geçerli bir e-posta adresi olmalıdır (`EmailAddress`).
 *
 * - TwoFactorCode:
 *   - Boş olmamalıdır (`NotEmpty`).
 *   - 6 haneli olmalıdır (`Length(6)`).
 *   - Sadece rakamlardan oluşmalıdır (`Matches("^[0-9]*$")`).
 *
 * Kullanım:
 * - Bu sınıf, iki faktörlü kimlik doğrulama işlemi sırasında, kullanıcı tarafından gönderilen 2FA kodunun doğruluğunu
 *   kontrol etmek için kullanılır.
 * - Kullanıcıdan alınan e-posta adresi ve 2FA kodu, belirtilen kurallara göre doğrulanır.
 */
public class TwoFactorRequestValidator : AbstractValidator<TwoFactorRequest>
{
    public TwoFactorRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.TwoFactorCode)
            .NotEmpty().WithMessage("2FA code is required.")
            .Length(6).WithMessage("2FA code must be 6 digits.")
            .Matches("^[0-9]*$").WithMessage("2FA code must contain only numbers.");
    }
}