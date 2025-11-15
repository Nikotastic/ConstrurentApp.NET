using AutoMapper;
using Firmness.Admin.Web.ViewModels.Sales;
using Firmness.Domain.Entities;

namespace Firmness.Web.Mappings;

// Mapping profiles for Sale - Web (ViewModels)
public class SaleViewModelMappingProfile : Profile
{
    public SaleViewModelMappingProfile()
    {
        // Entity to FormViewModel (for viewing/editing)
        CreateMap<Sale, SaleFormViewModel>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName))
            .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer.Email))
            .ForMember(dest => dest.Customers, opt => opt.Ignore()); // Se llena en el controlador

        // FormViewModel to Entity (for create)
        CreateMap<SaleFormViewModel, Sale>()
            .ConstructUsing(vm => new Sale(vm.CustomerId))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Items, opt => opt.Ignore());
    }
}

