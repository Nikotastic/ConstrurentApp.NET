﻿﻿using AutoMapper;
using Firmness.Domain.DTOs.Vehicle;
using Firmness.Domain.Entities;

namespace Firmness.Application.Mappings;

// Mapping profiles for VehicleRental
public class VehicleRentalMappingProfile : Profile
{
    public VehicleRentalMappingProfile()
    {
        // Entity to DTO
        CreateMap<VehicleRental, VehicleRentalDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : string.Empty))
            .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Email : string.Empty))
            .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Phone : string.Empty))
            .ForMember(dest => dest.VehicleDisplayName, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.DisplayName : string.Empty))
            .ForMember(dest => dest.VehicleBrand, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.Brand : string.Empty))
            .ForMember(dest => dest.VehicleModel, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.Model : string.Empty))
            .ForMember(dest => dest.VehicleLicensePlate, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.LicensePlate : string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
            .ForMember(dest => dest.DurationInDays, opt => opt.MapFrom(src => src.DurationInDays))
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.IsOverdue));

        // Entity to List DTO (simplified)
        CreateMap<VehicleRental, VehicleRentalListDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : string.Empty))
            .ForMember(dest => dest.VehicleDisplayName, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.DisplayName : string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.IsOverdue));

        // Create DTO to Entity
        CreateMap<CreateVehicleRentalDto, VehicleRental>()
            .ConstructUsing(src => new VehicleRental(
                src.CustomerId,
                src.VehicleId,
                src.StartDate,
                src.EstimatedReturnDate,
                src.RentalRate
            ))
            .ForMember(dest => dest.RentalPeriodType, opt => opt.MapFrom(src => src.RentalPeriodType))
            .ForMember(dest => dest.Deposit, opt => opt.MapFrom(src => src.Deposit))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
            .ForMember(dest => dest.Tax, opt => opt.MapFrom(src => src.Tax))
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
            .ForMember(dest => dest.PickupLocation, opt => opt.MapFrom(src => src.PickupLocation))
            .ForMember(dest => dest.ReturnLocation, opt => opt.MapFrom(src => src.ReturnLocation))
            .ForMember(dest => dest.InitialHours, opt => opt.MapFrom(src => src.InitialHours))
            .ForMember(dest => dest.InitialMileage, opt => opt.MapFrom(src => src.InitialMileage))
            .ForMember(dest => dest.InitialCondition, opt => opt.MapFrom(src => src.InitialCondition))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Vehicle, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

        // Update DTO to Entity (only map non-null properties)
        CreateMap<UpdateVehicleRentalDto, VehicleRental>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
    }
}

