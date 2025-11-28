using Firmness.Domain.Common;
using Firmness.Application.DTOs.Vehicle;

namespace Firmness.Application.Interfaces;

public interface IVehicleService
{
    Task<Result<IEnumerable<VehicleDto>>> GetAllVehiclesAsync();
    Task<Result<IEnumerable<VehicleDto>>> GetAvailableVehiclesAsync();
    Task<Result<VehicleDto>> GetVehicleByIdAsync(Guid id);
    Task<Result<VehicleDto>> CreateVehicleAsync(CreateVehicleDto createDto);
    Task<Result<VehicleDto>> UpdateVehicleAsync(Guid id, UpdateVehicleDto updateDto);
    Task<Result> DeleteVehicleAsync(Guid id);
    Task<Result> UpdateVehicleStatusAsync(Guid id, string status);
    Task<Result<IEnumerable<VehicleDto>>> GetVehiclesByTypeAsync(string vehicleType);
    Task<Result<VehicleAvailabilityResponseDto>> CheckAvailabilityAsync(Guid vehicleId, DateTime startDate, DateTime endDate);
}

