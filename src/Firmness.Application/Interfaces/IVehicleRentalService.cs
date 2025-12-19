using Firmness.Domain.Common;
using Firmness.Application.DTOs.Vehicle;
using Firmness.Domain.Enums;

namespace Firmness.Application.Interfaces;

public interface IVehicleRentalService
{
    Task<Result<IEnumerable<VehicleRentalDto>>> GetAllRentalsAsync();
    Task<Result<IEnumerable<VehicleRentalDto>>> GetAllRentalsWithDetailsAsync();
    Task<Result<VehicleRentalDto>> GetRentalByIdAsync(Guid id);
    Task<Result<VehicleRentalDto>> GetRentalByIdWithDetailsAsync(Guid id);
    Task<Result<IEnumerable<VehicleRentalDto>>> GetRentalsByCustomerIdAsync(Guid customerId);
    Task<Result<IEnumerable<VehicleRentalDto>>> GetRentalsByVehicleIdAsync(Guid vehicleId);
    Task<Result<IEnumerable<VehicleRentalDto>>> GetRentalsByStatusAsync(RentalStatus status);
    Task<Result<VehicleRentalDto>> CreateRentalAsync(CreateVehicleRentalDto createDto);
    Task<Result<VehicleRentalDto>> UpdateRentalAsync(Guid id, UpdateVehicleRentalDto updateDto);
    Task<Result> DeleteRentalAsync(Guid id);
    Task<Result<VehicleRentalDto>> CompleteRentalAsync(Guid id, CompleteVehicleRentalDto completeDto);
    Task<Result<VehicleRentalDto>> CancelRentalAsync(Guid id, string cancellationReason);
    Task<Result> ProcessPaymentAsync(Guid id, decimal amount);
    Task<Result> ReturnDepositAsync(Guid id);
}

