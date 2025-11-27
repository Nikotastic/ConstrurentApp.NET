using Firmness.Application.AI.DTOs;
using Firmness.Domain.DTOs.Vehicle;

namespace Firmness.Application.AI.Mappers;

// Mapper to convert VehicleDto to VehicleSummaryDto
public static class VehicleMapper
{
    public static VehicleSummaryDto ToSummary(VehicleDto vehicle)
    {
        if (vehicle == null)
            throw new ArgumentNullException(nameof(vehicle));

        return new VehicleSummaryDto
        {
            Id = vehicle.Id,
            DisplayName = vehicle.DisplayName,
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            VehicleType = vehicle.VehicleType,
            Year = vehicle.Year,
            DailyRate = vehicle.DailyRate,
            Status = vehicle.Status,
            IsAvailable = vehicle.IsAvailableForRent,
            Specifications = vehicle.Specifications
        };
    }

    public static List<VehicleSummaryDto> ToSummaryList(IEnumerable<VehicleDto> vehicles)
    {
        return vehicles?.Select(ToSummary).ToList() ?? new List<VehicleSummaryDto>();
    }
}