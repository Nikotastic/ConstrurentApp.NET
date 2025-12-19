using AutoMapper;
using Firmness.Application.Interfaces;
using Firmness.Domain.Common;
using Firmness.Application.DTOs.Vehicle;
using Firmness.Domain.Entities;
using Firmness.Domain.Enums;
using Firmness.Domain.Interfaces;

namespace Firmness.Application.Services;

// Service for vehicles
public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IVehicleRentalRepository _rentalRepository;
    private readonly IMapper _mapper;

    public VehicleService(
        IVehicleRepository vehicleRepository,
        IVehicleRentalRepository rentalRepository,
        IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _rentalRepository = rentalRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<VehicleDto>>> GetAllVehiclesAsync()
    {
        try
        {
            var vehicles = await _vehicleRepository.GetAllAsync();
            var vehicleDtos = _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
            return Result<IEnumerable<VehicleDto>>.Success(vehicleDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<VehicleDto>>.Failure(
                $"An error occurred while retrieving vehicles: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }

    public async Task<Result<IEnumerable<VehicleDto>>> GetAvailableVehiclesAsync()
    {
        try
        {
            var vehicles = await _vehicleRepository.GetAvailableVehiclesAsync();
            var vehicleDtos = _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
            return Result<IEnumerable<VehicleDto>>.Success(vehicleDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<VehicleDto>>.Failure(
                $"An error occurred while retrieving available vehicles: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }

    public async Task<Result<VehicleDto>> GetVehicleByIdAsync(Guid id)
    {
        try
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null)
                return Result<VehicleDto>.Failure("Vehicle not found.", ErrorCodes.NotFound);

            var vehicleDto = _mapper.Map<VehicleDto>(vehicle);
            return Result<VehicleDto>.Success(vehicleDto);
        }
        catch (Exception ex)
        {
            return Result<VehicleDto>.Failure(
                $"An error occurred while retrieving the vehicle: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }

    public async Task<Result<VehicleDto>> CreateVehicleAsync(CreateVehicleDto createDto)
    {
        try
        {
            // Validate license plate uniqueness
            var existing = await _vehicleRepository.GetByLicensePlateAsync(createDto.LicensePlate);
            if (existing != null)
                return Result<VehicleDto>.Failure("A vehicle with this license plate already exists.", ErrorCodes.Validation);

            var vehicle = _mapper.Map<Vehicle>(createDto);
            await _vehicleRepository.AddAsync(vehicle);

            var vehicleDto = _mapper.Map<VehicleDto>(vehicle);
            return Result<VehicleDto>.Success(vehicleDto);
        }
        catch (Exception ex)
        {
            return Result<VehicleDto>.Failure(
                $"An error occurred while creating the vehicle: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }

    public async Task<Result<VehicleDto>> UpdateVehicleAsync(Guid id, UpdateVehicleDto updateDto)
    {
        try
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null)
                return Result<VehicleDto>.Failure("Vehicle not found.", ErrorCodes.NotFound);

            // Validate license plate uniqueness if it's being changed
            if (!string.IsNullOrWhiteSpace(updateDto.LicensePlate) && updateDto.LicensePlate != vehicle.LicensePlate)
            {
                var existing = await _vehicleRepository.GetByLicensePlateAsync(updateDto.LicensePlate);
                if (existing != null)
                    return Result<VehicleDto>.Failure("A vehicle with this license plate already exists.", ErrorCodes.Validation);
            }

            _mapper.Map(updateDto, vehicle);
            await _vehicleRepository.UpdateAsync(vehicle);

            var vehicleDto = _mapper.Map<VehicleDto>(vehicle);
            return Result<VehicleDto>.Success(vehicleDto);
        }
        catch (Exception ex)
        {
            return Result<VehicleDto>.Failure(
                $"An error occurred while updating the vehicle: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }

    public async Task<Result> DeleteVehicleAsync(Guid id)
    {
        try
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null)
                return Result.Failure("Vehicle not found.", ErrorCodes.NotFound);

            // Check if vehicle has active rentals
            var rentals = await _rentalRepository.GetByVehicleIdAsync(id);
            var activeRentals = rentals.Where(r => 
                r.Status == RentalStatus.Active || 
                r.Status == RentalStatus.Pending).ToList();

            if (activeRentals.Any())
                return Result.Failure("Cannot delete vehicle with active rentals.", ErrorCodes.Validation);

            await _vehicleRepository.DeleteAsync(id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(
                $"An error occurred while deleting the vehicle: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }

    public async Task<Result> UpdateVehicleStatusAsync(Guid id, string status)
    {
        try
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null)
                return Result.Failure("Vehicle not found.", ErrorCodes.NotFound);

            if (!Enum.TryParse<VehicleStatus>(status, true, out var vehicleStatus))
                return Result.Failure("Invalid vehicle status.", ErrorCodes.Validation);

            vehicle.Status = vehicleStatus;
            await _vehicleRepository.UpdateAsync(vehicle);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(
                $"An error occurred while updating vehicle status: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }

    public async Task<Result<IEnumerable<VehicleDto>>> GetVehiclesByTypeAsync(string vehicleType)
    {
        try
        {
            if (!Enum.TryParse<VehicleType>(vehicleType, true, out var type))
                return Result<IEnumerable<VehicleDto>>.Failure("Invalid vehicle type.", ErrorCodes.Validation);

            var vehicles = await _vehicleRepository.GetByTypeAsync(type);
            var vehicleDtos = _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
            return Result<IEnumerable<VehicleDto>>.Success(vehicleDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<VehicleDto>>.Failure(
                $"An error occurred while retrieving vehicles by type: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }

    public async Task<Result<VehicleAvailabilityResponseDto>> CheckAvailabilityAsync(Guid vehicleId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle == null)
                return Result<VehicleAvailabilityResponseDto>.Failure("Vehicle not found.", ErrorCodes.NotFound);

            var rentals = await _rentalRepository.GetByVehicleIdAsync(vehicleId);
            
            // Check if there are any overlapping rentals
            var conflictingRentals = rentals.Where(r =>
                (r.Status == RentalStatus.Active || r.Status == RentalStatus.Pending) &&
                ((r.StartDate <= endDate && r.EstimatedReturnDate >= startDate) ||
                 (r.ActualReturnDate.HasValue && r.StartDate <= endDate && r.ActualReturnDate >= startDate))
            ).ToList();

            var availabilityDto = new VehicleAvailabilityResponseDto
            {
                VehicleId = vehicleId,
                VehicleDisplayName = vehicle.DisplayName,
                IsAvailable = !conflictingRentals.Any() && vehicle.Status == VehicleStatus.Available,
                Reason = conflictingRentals.Any() ? "Vehicle has conflicting rentals" : 
                         vehicle.Status != VehicleStatus.Available ? $"Vehicle is {vehicle.Status}" : null,
                ConflictingRentals = conflictingRentals.Select(r => new ConflictingRentalDto
                {
                    RentalId = r.Id,
                    StartDate = r.StartDate,
                    EstimatedReturnDate = r.ActualReturnDate ?? r.EstimatedReturnDate,
                    CustomerName = r.Customer?.FullName ?? "Unknown"
                }).ToList()
            };

            return Result<VehicleAvailabilityResponseDto>.Success(availabilityDto);
        }
        catch (Exception ex)
        {
            return Result<VehicleAvailabilityResponseDto>.Failure(
                $"An error occurred while checking availability: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }
}

