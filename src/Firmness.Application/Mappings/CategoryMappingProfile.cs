using AutoMapper;
using Firmness.Application.DTOs;
using Firmness.Domain.Entities;

namespace Firmness.Application.Mappings;

// Mapping profiles for Category - API (DTOs)
public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        // Entity to DTO
        CreateMap<Category, CategoryDto>();

        // Create DTO to Entity
        CreateMap<CreateCategoryDto, Category>()
            .ConstructUsing(src => new Category(src.Name, src.Description));

        // Update DTO to Entity (just map non null properties)
        CreateMap<UpdateCategoryDto, Category>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
    }
}
