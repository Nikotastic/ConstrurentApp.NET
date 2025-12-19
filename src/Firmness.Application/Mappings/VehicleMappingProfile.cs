using AutoMapper;
using Firmness.Application.DTOs.Vehicle;
using Firmness.Domain.Entities;

namespace Firmness.Application.Mappings;

// Mapping profiles for Vehicle
public class VehicleMappingProfile : Profile
{
    public VehicleMappingProfile()
    {
        // Entity to DTO
        CreateMap<Vehicle, VehicleDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType.ToString()))
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
            .ForMember(dest => dest.IsAvailableForRent, opt => opt.MapFrom(src => src.IsAvailableForRent))
            .ForMember(dest => dest.NeedsMaintenance, opt => opt.MapFrom(src => src.NeedsMaintenance));

        // Create DTO to Entity
        CreateMap<CreateVehicleDto, Vehicle>()
            .ConstructUsing(src => new Vehicle(
                src.Brand,
                src.Model,
                src.Year,
                src.LicensePlate,
                src.VehicleType
            ))
            .ForMember(dest => dest.HourlyRate, opt => opt.MapFrom(src => src.HourlyRate))
            .ForMember(dest => dest.DailyRate, opt => opt.MapFrom(src => src.DailyRate))
            .ForMember(dest => dest.WeeklyRate, opt => opt.MapFrom(src => src.WeeklyRate))
            .ForMember(dest => dest.MonthlyRate, opt => opt.MapFrom(src => src.MonthlyRate))
            .ForMember(dest => dest.CurrentHours, opt => opt.MapFrom(src => src.CurrentHours))
            .ForMember(dest => dest.CurrentMileage, opt => opt.MapFrom(src => src.CurrentMileage))
            .ForMember(dest => dest.Specifications, opt => opt.MapFrom(src => src.Specifications))
            .ForMember(dest => dest.SerialNumber, opt => opt.MapFrom(src => src.SerialNumber))
            .ForMember(dest => dest.MaintenanceHoursInterval, opt => opt.MapFrom(src => src.MaintenanceHoursInterval))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.DocumentsUrl, opt => opt.MapFrom(src => src.DocumentsUrl))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));

        // Update DTO to Entity (only map non-null properties)
        CreateMap<UpdateVehicleDto, Vehicle>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
    }
}

