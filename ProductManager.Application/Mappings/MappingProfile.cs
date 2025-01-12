// ProductManager.Application/Mappings/MappingProfile.cs
using AutoMapper;
using ProductManager.Core.DTOs;
using ProductManager.Core.Entities;
using ProductManager.Core.Identity.DTOs;
using ProductManager.Core.Identity.Entities;

namespace ProductManager.Application.Mappings;

/*
 * MappingProfile, AutoMapper kütüphanesini kullanarak nesneler arasında dönüşüm kurallarını belirler.
 * Bu sınıf, özellikle Product ve Identity nesneleri arasında dönüşümleri sağlar.
 * Profil içerisinde yer alan kurallar, DTO (Data Transfer Object) ve Entity (veri model) nesneleri arasında verilerin
 * nasıl aktarılacağını belirler.
 *
 * Mapping işlemleri:
 *
 * - Ürün (Product) ve DTO'ları arasında dönüşüm:
 *   - Product -> ProductDto
 *   - ProductDto -> Product
 *   - CreateProductDto -> Product (Bazı özellikler hariç tutulur: Id, CreatedDate, UpdatedDate, IsActive)
 *
 * - Kimlik (Identity) işlemleri için dönüşüm:
 *   - RegisterRequest -> ApplicationUser (Email, CreatedDate, ve IsActive gibi özellikler ile eşleştirme yapılır. Diğer
 *     kimlik özellikleri hariç tutulur)
 *
 * Bu yapı, veritabanı ve API arasındaki veri transferinin daha kolay ve hatasız yapılmasına yardımcı olur.
 */
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Existing Product mappings
        CreateMap<Product, ProductDto>();
        CreateMap<ProductDto, Product>();
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore());

        // Identity mappings
        CreateMap<RegisterRequest, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore())
            .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
            .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.Ignore())
            .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
            .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
            .ForMember(dest => dest.LockoutEnabled, opt => opt.Ignore())
            .ForMember(dest => dest.AccessFailedCount, opt => opt.Ignore());
    }
}