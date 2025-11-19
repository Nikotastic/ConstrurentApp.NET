using AutoMapper;
using Firmness.Domain.DTOs;
using Firmness.Domain.Entities;

namespace Firmness.Application.Mappings;

public class SaleMappingProfile : Profile
{
    public SaleMappingProfile()
    {
        // Entity to DTO
        CreateMap<Sale, SaleDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<SaleItem, SaleItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductSKU, opt => opt.MapFrom(src => src.Product.SKU))
            .ForMember(dest => dest.LineTotal, opt => opt.MapFrom(src => src.LineaTotal));

        // Create DTO to Entity
        CreateMap<CreateSaleDto, Sale>()
            .ConstructUsing(src => new Sale(src.CustomerId))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
            .ForMember(dest => dest.Tax, opt => opt.MapFrom(src => src.Tax))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));

        CreateMap<CreateSaleItemDto, SaleItem>()
            .ConstructUsing(src => new SaleItem(
                src.ProductId,
                src.Quantity,
                src.UnitPrice
            ));

        // Update DTOs
        CreateMap<UpdateSaleDto, Sale>()
            .ForMember(dest => dest.Items, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));

        CreateMap<UpdateSaleItemDto, SaleItem>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice));
    }
}
