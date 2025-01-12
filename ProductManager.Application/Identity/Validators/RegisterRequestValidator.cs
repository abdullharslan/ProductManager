// ProductManager.Application/Identity/Validators/RegisterRequestValidator.cs
using FluentValidation;
using ProductManager.Core.Identity.DTOs;

namespace ProductManager.Application.Identity.Validators;

/*
 * RegisterRequestValidator, kullanıcı kaydını doğrulamak için kullanılan bir sınıftır. FluentValidation kütüphanesini
 * kullanarak, kullanıcıdan alınan bilgiler üzerinde doğrulama yapar. Bu doğrulamalar, kullanıcı kaydını güvenli ve
 * doğru bir şekilde gerçekleştirmek için gereklidir.
 *
 * Kurallar:
 *
 * - FirstName:
 *   - Boş olmamalıdır (`NotEmpty`).
 *   - En fazla 50 karakter uzunluğunda olmalıdır (`MaximumLength`).
 *
 * - LastName:
 *   - Boş olmamalıdır (`NotEmpty`).
 *   - En fazla 50 karakter uzunluğunda olmalıdır (`MaximumLength`).
 *
 * - Email:
 *   - Boş olmamalıdır (`NotEmpty`).
 *   - Geçerli bir e-posta adresi olmalıdır (`EmailAddress`).
 *
 * - Password:
 *   - Boş olmamalıdır (`NotEmpty`).
 *   - En az 8 karakter uzunluğunda olmalıdır (`MinimumLength`).
 *   - En az bir büyük harf içermelidir (`Matches("[A-Z]")`).
 *   - En az bir küçük harf içermelidir (`Matches("[a-z]")`).
 *   - En az bir rakam içermelidir (`Matches("[0-9]")`).
 *   - En az bir özel karakter içermelidir (`Matches("[^a-zA-Z0-9]")`).
 *
 * - ConfirmPassword:
 *   - Boş olmamalıdır (`NotEmpty`).
 *   - Password ile aynı olmalıdır (`Equal`).
 *
 * Kullanım:
 * - Bu sınıf, kullanıcı kayıt işlemi sırasında doğrulama yapmak için kullanılır.
 * - Kullanıcıdan alınan kayıt bilgileri, bu doğrulama kurallarına göre kontrol edilir.
 */
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Password confirmation is required.")
            .Equal(x => x.Password).WithMessage("Passwords do not match.");
    }
}