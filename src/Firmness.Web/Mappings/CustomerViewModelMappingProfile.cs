using AutoMapper;
using Firmness.Web.ViewModels.Customer;
using Firmness.Domain.Entities;

namespace Firmness.Web.Mappings;

public class CustomerViewModelMappingProfile : Profile
{
    public CustomerViewModelMappingProfile()
    {
        // Customer -> CustomerFormViewModel
        CreateMap<Customer, CustomerFormViewModel>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
        
        // CustomerFormViewModel -> Customer
        CreateMap<CustomerFormViewModel, Customer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IdentityUserId, opt => opt.Ignore())
            .ForMember(dest => dest.Sales, opt => opt.Ignore());
    }
}
