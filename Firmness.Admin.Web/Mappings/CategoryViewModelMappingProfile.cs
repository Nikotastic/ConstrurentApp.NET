using AutoMapper;
using Firmness.Web.ViewModels.Category;
using Firmness.Domain.Entities;

namespace Firmness.Web.Mappings;

// Mapping profiles for Category - Web (ViewModels)
public class CategoryViewModelMappingProfile : Profile
{
    public CategoryViewModelMappingProfile()
    {
        // Entity to FormViewModel
        CreateMap<Category, CategoryFormViewModel>()
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));

        // FormViewModel to Entity
        CreateMap<CategoryFormViewModel, Category>()
            .ConstructUsing(vm => new Category(vm.Name, vm.Description ?? string.Empty))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}

