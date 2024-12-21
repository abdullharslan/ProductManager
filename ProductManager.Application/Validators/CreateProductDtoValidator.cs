// ProductManager.Application/Validators/CreateProductDtoValidator.cs

using FluentValidation;
using ProductManager.Core.DTOs;

namespace ProductManager.Application.Validators;

/*
 * CreateProductDtoValidator sınıfı, ürün oluşturma ve güncelleme işlemlerinde
 * kullanılan validasyon kurallarını tanımlar.
 *
 * FluentValidation kütüphanesi ile:
 * - İş kurallarını açık ve okunabilir şekilde tanımlar
 * - Validasyon mantığını servis katmanından ayırır
 * - Yeni kurallar eklemek için esneklik sağlar
 */
public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");
    }
}