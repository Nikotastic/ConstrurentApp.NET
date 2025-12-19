using AutoMapper;
using Firmness.Application.DTOs;
using Firmness.Domain.Entities;

namespace Firmness.Application.Mappings;


// Mapping profiles for Product - API (DTOs)

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        // Entity to DTO
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));

        // Create DTO to Entity
        CreateMap<CreateProductDto, Product>()
            .ConstructUsing(src => new Product(
                src.SKU,
                src.Name,
                src.Description,
                src.Price,
                src.ImageUrl,
                src.Stock
            ))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.MinStock, opt => opt.MapFrom(src => src.MinStock))
            .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Cost))
            .ForMember(dest => dest.Barcode, opt => opt.MapFrom(src => src.Barcode));

        // Update DTO to Entity (just map non null properties)
        CreateMap<UpdateProductDto, Product>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
    }
}
