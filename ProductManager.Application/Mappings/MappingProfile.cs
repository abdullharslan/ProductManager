// ProductManager.Application/Mappings/MappingProfile.cs

using AutoMapper;
using ProductManager.Core.DTOs;
using ProductManager.Core.Entities;

namespace ProductManager.Application.Mappings;

/*
 * MappingProfile sınıfı, AutoMapper kütüphanesi için entity ve DTO'lar arasındaki
 * dönüşüm kurallarını tanımlar.
 *
 * Bu profil:
 * - Entity -> DTO
 * - DTO -> Entity
 * dönüşümlerini yapılandırır.
 */
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<ProductDto, Product>();
        
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore());
    }
}