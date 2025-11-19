using AutoMapper;
using Firmness.Web.ViewModels;
using Firmness.Web.ViewModels.Product;
using Firmness.Domain.Entities;

namespace Firmness.Web.Mappings;

// Mapping profiles for Product - Web (ViewModels)
public class ProductViewModelMappingProfile : Profile
{
    public ProductViewModelMappingProfile()
    {
        // Entity to FormViewModel
        CreateMap<Product, ProductFormViewModel>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));

        // FormViewModel to Entity
        CreateMap<ProductFormViewModel, Product>()
            .ConstructUsing(vm => new Product(
                vm.SKU, 
                vm.Name, 
                vm.Description ?? string.Empty, 
                vm.Price ?? 0, 
                vm.ImageUrl ?? string.Empty, 
                vm.Stock ?? 0))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.MinStock, opt => opt.MapFrom(src => src.MinStock))
            .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Cost))
            .ForMember(dest => dest.Barcode, opt => opt.MapFrom(src => src.Barcode))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.SaleItems, opt => opt.Ignore());

        // Entity to ProductViewModel
        CreateMap<Product, ProductViewModel>()
            .ForMember(dest => dest.Age, opt => opt.Ignore())
            .ForMember(dest => dest.AgeValue, opt => opt.Ignore());

        // ProductViewModel to Entity
        CreateMap<ProductViewModel, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.Stock, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.MinStock, opt => opt.Ignore())
            .ForMember(dest => dest.Cost, opt => opt.Ignore())
            .ForMember(dest => dest.Barcode, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.SaleItems, opt => opt.Ignore());

        // Entity to ProductListItemViewModel
        CreateMap<Product, ProductListItemViewModel>()
            .ForMember(dest => dest.PriceFormatted, opt => opt.MapFrom(src => src.Price.ToString("C")));

        // ProductListItemViewModel to Entity (if needed)
        CreateMap<ProductListItemViewModel, Product>()
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.SKU))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Description, opt => opt.Ignore())
            .ForMember(dest => dest.Price, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.Stock, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.MinStock, opt => opt.Ignore())
            .ForMember(dest => dest.Cost, opt => opt.Ignore())
            .ForMember(dest => dest.Barcode, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.SaleItems, opt => opt.Ignore());
    }
}

