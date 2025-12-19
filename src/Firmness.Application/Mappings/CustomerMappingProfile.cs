using AutoMapper;
using Firmness.Application.DTOs;
using Firmness.Domain.Entities;

namespace Firmness.Application.Mappings;

// Mapping profiles for Customer - API (DTOs)

public class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        // Entity to DTO (information limit)
        CreateMap<Customer, CustomerDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));

        // Entity to DetailDTO (full info - just Admin)
        CreateMap<Customer, CustomerDetailDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));

        // Create DTO to Entity
        CreateMap<CreateCustomerDto, Customer>()
            .ConstructUsing(src => new Customer(
                src.FirstName,
                src.LastName,
                src.Email
            ))
            .ForMember(dest => dest.Document, opt => opt.MapFrom(src => src.Document))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

        // Update DTO to Entity (just map non null properties)
        CreateMap<UpdateCustomerDto, Customer>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
