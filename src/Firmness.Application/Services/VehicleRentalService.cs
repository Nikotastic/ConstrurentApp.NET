﻿﻿using AutoMapper;
using Firmness.Application.Interfaces;
using Firmness.Domain.Common;
using Firmness.Domain.DTOs.Vehicle;
using Firmness.Domain.Entities;
using Firmness.Domain.Enums;
using Firmness.Domain.Interfaces;

namespace Firmness.Application.Services;

// Service for vehicle rentals
public class VehicleRentalService : IVehicleRentalService
{
    private readonly IVehicleRentalRepository _rentalRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public VehicleRentalService(
        IVehicleRentalRepository rentalRepository,
        IVehicleRepository vehicleRepository,
        ICustomerRepository customerRepository,
        INotificationService @object,
        IMapper mapper)
    {
        _rentalRepository = rentalRepository;
        _vehicleRepository = vehicleRepository;
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<VehicleRentalDto>>> GetAllRentalsAsync()
    {
        try
        {
            var rentals = await _rentalRepository.GetAllAsync();
            var rentalDtos = _mapper.Map<IEnumerable<VehicleRentalDto>>(rentals);
            return Result<IEnumerable<VehicleRentalDto>>.Success(rentalDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<VehicleRentalDto>>.Failure(
                $"An error occurred while retrieving rentals: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }

    public async Task<Result<IEnumerable<VehicleRentalDto>>> GetAllRentalsWithDetailsAsync()
    {
        try
        {
            var rentals = await _rentalRepository.GetAllWithDetailsAsync();
            var rentalDtos = _mapper.Map<IEnumerable<VehicleRentalDto>>(rentals);
            return Result<IEnumerable<VehicleRentalDto>>.Success(rentalDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<VehicleRentalDto>>.Failure(
                $"An error occurred while retrieving rentals with details: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }

    public async Task<Result<VehicleRentalDto>> GetRentalByIdAsync(Guid id)
    {
        try
        {
            var rental = await _rentalRepository.GetByIdAsync(id);
            if (rental == null)
                return Result<VehicleRentalDto>.Failure("Rental not found.", ErrorCodes.NotFound);

            var rentalDto = _mapper.Map<VehicleRentalDto>(rental);
            return Result<VehicleRentalDto>.Success(rentalDto);
        }
        catch (Exception ex)
        {
            return Result<VehicleRentalDto>.Failure(
                $"An error occurred while retrieving the rental: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }

    public async Task<Result<VehicleRentalDto>> GetRentalByIdWithDetailsAsync(Guid id)
    {
        try
        {
            var rental = await _rentalRepository.GetByIdWithDetailsAsync(id);
            if (rental == null)
                return Result<VehicleRentalDto>.Failure("Rental not found.", ErrorCodes.NotFound);

            var rentalDto = _mapper.Map<VehicleRentalDto>(rental);
            return Result<VehicleRentalDto>.Success(rentalDto);
        }
        catch (Exception ex)
        {
            return Result<VehicleRentalDto>.Failure(
                $"An error occurred while retrieving the rental: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }

    public async Task<Result<IEnumerable<VehicleRentalDto>>> GetRentalsByCustomerIdAsync(Guid customerId)
    {
        try
        {
            var rentals = await _rentalRepository.GetByCustomerIdAsync(customerId);
            var rentalDtos = _mapper.Map<IEnumerable<VehicleRentalDto>>(rentals);
            return Result<IEnumerable<VehicleRentalDto>>.Success(rentalDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<VehicleRentalDto>>.Failure(
                $"An error occurred while retrieving rentals by customer: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }

    public async Task<Result<IEnumerable<VehicleRentalDto>>> GetRentalsByVehicleIdAsync(Guid vehicleId)
    {
        try
        {
            var rentals = await _rentalRepository.GetByVehicleIdAsync(vehicleId);
            var rentalDtos = _mapper.Map<IEnumerable<VehicleRentalDto>>(rentals);
            return Result<IEnumerable<VehicleRentalDto>>.Success(rentalDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<VehicleRentalDto>>.Failure(
                $"An error occurred while retrieving rentals by vehicle: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }

    public async Task<Result<IEnumerable<VehicleRentalDto>>> GetRentalsByStatusAsync(RentalStatus status)
    {
        try
        {
            var rentals = await _rentalRepository.GetByStatusAsync(status);
            var rentalDtos = _mapper.Map<IEnumerable<VehicleRentalDto>>(rentals);
            return Result<IEnumerable<VehicleRentalDto>>.Success(rentalDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<VehicleRentalDto>>.Failure(
                $"An error occurred while retrieving rentals by status: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }

    public async Task<Result<VehicleRentalDto>> CreateRentalAsync(CreateVehicleRentalDto createDto)
    {
        try
        {
            // Validate customer exists
            var customer = await _customerRepository.GetByIdAsync(createDto.CustomerId);
            if (customer == null)
                return Result<VehicleRentalDto>.Failure("Customer not found.", ErrorCodes.NotFound);

            // Validate vehicle exists
            var vehicle = await _vehicleRepository.GetByIdAsync(createDto.VehicleId);
            if (vehicle == null)
                return Result<VehicleRentalDto>.Failure("Vehicle not found.", ErrorCodes.NotFound);

            // Check vehicle availability
            if (vehicle.Status != VehicleStatus.Available)
                return Result<VehicleRentalDto>.Failure("Vehicle is not available for rent.", ErrorCodes.Validation);

            // Check for overlapping rentals
            var existingRentals = await _rentalRepository.GetByVehicleIdAsync(createDto.VehicleId);
            var conflicting = existingRentals.Where(r =>
                (r.Status == RentalStatus.Active || r.Status == RentalStatus.Pending) &&
                ((r.StartDate <= createDto.EstimatedReturnDate && r.EstimatedReturnDate >= createDto.StartDate))
            ).ToList();

            if (conflicting.Any())
                return Result<VehicleRentalDto>.Failure("Vehicle is already rented for this period.", ErrorCodes.Validation);

            // Convert dates to UTC if needed
            var startDateUtc = createDto.StartDate.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(createDto.StartDate, DateTimeKind.Utc) 
                : createDto.StartDate.ToUniversalTime();
                
            var estimatedReturnDateUtc = createDto.EstimatedReturnDate.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(createDto.EstimatedReturnDate, DateTimeKind.Utc) 
                : createDto.EstimatedReturnDate.ToUniversalTime();

            // Create rental using constructor
            var rental = new VehicleRental(
                createDto.CustomerId,
                createDto.VehicleId,
                startDateUtc,
                estimatedReturnDateUtc,
                createDto.RentalRate
            );
            
            // Set additional properties
            rental.RentalPeriodType = createDto.RentalPeriodType ?? "Daily";
            rental.Deposit = createDto.Deposit;
            rental.Discount = createDto.Discount;
            rental.Tax = createDto.Tax;
            rental.PaymentMethod = createDto.PaymentMethod;
            rental.PickupLocation = createDto.PickupLocation;
            rental.ReturnLocation = createDto.ReturnLocation;
            rental.InitialHours = createDto.InitialHours;
            rental.InitialMileage = createDto.InitialMileage;
            rental.InitialCondition = createDto.InitialCondition;
            rental.Notes = createDto.Notes;
            
            // Calculate totals
            rental.Subtotal = CalculateSubtotal(rental.RentalRate, rental.StartDate, rental.EstimatedReturnDate, rental.RentalPeriodType);
            rental.TotalAmount = rental.Subtotal + rental.Tax - rental.Discount;
            rental.PaidAmount = 0;
            rental.PendingAmount = rental.TotalAmount;
            rental.Status = RentalStatus.Pending;

            await _rentalRepository.AddAsync(rental);

            // Update vehicle status
            vehicle.Status = VehicleStatus.Rented;
            await _vehicleRepository.UpdateAsync(vehicle);

            var rentalDto = _mapper.Map<VehicleRentalDto>(rental);
            return Result<VehicleRentalDto>.Success(rentalDto);
        }
        catch (Exception ex)
        {
            var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            return Result<VehicleRentalDto>.Failure(
                $"An error occurred while creating the rental: {innerMessage}", 
                ErrorCodes.ServerError);
        }
    }

    public async Task<Result<VehicleRentalDto>> UpdateRentalAsync(Guid id, UpdateVehicleRentalDto updateDto)
    {
        try
        {
            var rental = await _rentalRepository.GetByIdAsync(id);
            if (rental == null)
                return Result<VehicleRentalDto>.Failure("Rental not found.", ErrorCodes.NotFound);

            _mapper.Map(updateDto, rental);

            // Recalculate totals if relevant fields changed
            if (updateDto.RentalRate.HasValue || updateDto.StartDate.HasValue || 
                updateDto.EstimatedReturnDate.HasValue || updateDto.Tax.HasValue || updateDto.Discount.HasValue)
            {
                rental.Subtotal = CalculateSubtotal(rental.RentalRate, rental.StartDate, rental.EstimatedReturnDate, rental.RentalPeriodType);
                rental.TotalAmount = rental.Subtotal + rental.Tax - rental.Discount;
                rental.CalculatePendingAmount();
            }

            await _rentalRepository.UpdateAsync(rental);

            var rentalDto = _mapper.Map<VehicleRentalDto>(rental);
            return Result<VehicleRentalDto>.Success(rentalDto);
        }
        catch (Exception ex)
        {
            return Result<VehicleRentalDto>.Failure(
                $"An error occurred while updating the rental: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }

    public async Task<Result> DeleteRentalAsync(Guid id)
    {
        try
        {
            var rental = await _rentalRepository.GetByIdAsync(id);
            if (rental == null)
                return Result.Failure("Rental not found.", ErrorCodes.NotFound);

            if (rental.Status == RentalStatus.Active)
                return Result.Failure("Cannot delete an active rental.", ErrorCodes.Validation);

            await _rentalRepository.DeleteAsync(id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(
                $"An error occurred while deleting the rental: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }

    public async Task<Result<VehicleRentalDto>> CompleteRentalAsync(Guid id, CompleteVehicleRentalDto completeDto)
    {
        try
        {
            var rental = await _rentalRepository.GetByIdWithDetailsAsync(id);
            if (rental == null)
                return Result<VehicleRentalDto>.Failure("Rental not found.", ErrorCodes.NotFound);

            if (rental.Status != RentalStatus.Active)
                return Result<VehicleRentalDto>.Failure("Only active rentals can be completed.", ErrorCodes.Validation);

            // Complete the rental
            rental.CompleteRental(completeDto.ReturnDate);
            rental.FinalHours = completeDto.FinalHours;
            rental.FinalMileage = completeDto.FinalMileage;
            rental.FinalCondition = completeDto.FinalCondition;
            rental.DepositReturned = completeDto.DepositReturned;
            
            if (!string.IsNullOrWhiteSpace(completeDto.Notes))
                rental.Notes = completeDto.Notes;

            await _rentalRepository.UpdateAsync(rental);

            // Update vehicle status back to available
            if (rental.Vehicle != null)
            {
                rental.Vehicle.Status = VehicleStatus.Available;
                if (completeDto.FinalHours.HasValue)
                    rental.Vehicle.CurrentHours = completeDto.FinalHours;
                if (completeDto.FinalMileage.HasValue)
                    rental.Vehicle.CurrentMileage = completeDto.FinalMileage;
                
                await _vehicleRepository.UpdateAsync(rental.Vehicle);
            }

            var rentalDto = _mapper.Map<VehicleRentalDto>(rental);
            return Result<VehicleRentalDto>.Success(rentalDto);
        }
        catch (Exception ex)
        {
            return Result<VehicleRentalDto>.Failure(
                $"An error occurred while completing the rental: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }

    public async Task<Result<VehicleRentalDto>> CancelRentalAsync(Guid id, string cancellationReason)
    {
        try
        {
            var rental = await _rentalRepository.GetByIdWithDetailsAsync(id);
            if (rental == null)
                return Result<VehicleRentalDto>.Failure("Rental not found.", ErrorCodes.NotFound);

            if (rental.Status == RentalStatus.Completed || rental.Status == RentalStatus.Cancelled)
                return Result<VehicleRentalDto>.Failure("Cannot cancel a completed or already cancelled rental.", ErrorCodes.Validation);

            rental.CancelRental(cancellationReason);
            await _rentalRepository.UpdateAsync(rental);

            // Update vehicle status back to available
            var vehicle = await _vehicleRepository.GetByIdAsync(rental.VehicleId);
            if (vehicle != null)
            {
                vehicle.Status = VehicleStatus.Available;
                await _vehicleRepository.UpdateAsync(vehicle);
            }

            var rentalDto = _mapper.Map<VehicleRentalDto>(rental);
            return Result<VehicleRentalDto>.Success(rentalDto);
        }
        catch (Exception ex)
        {
            return Result<VehicleRentalDto>.Failure(
                $"An error occurred while cancelling the rental: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }

    public async Task<Result> ProcessPaymentAsync(Guid id, decimal amount)
    {
        try
        {
            var rental = await _rentalRepository.GetByIdAsync(id);
            if (rental == null)
                return Result.Failure("Rental not found.", ErrorCodes.NotFound);

            if (amount <= 0)
                return Result.Failure("Payment amount must be greater than zero.", ErrorCodes.Validation);

            rental.PaidAmount += amount;
            rental.CalculatePendingAmount();

            await _rentalRepository.UpdateAsync(rental);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(
                $"An error occurred while processing payment: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }

    public async Task<Result> ReturnDepositAsync(Guid id)
    {
        try
        {
            var rental = await _rentalRepository.GetByIdAsync(id);
            if (rental == null)
                return Result.Failure("Rental not found.", ErrorCodes.NotFound);

            if (rental.Status != RentalStatus.Completed)
                return Result.Failure("Deposit can only be returned for completed rentals.", ErrorCodes.Validation);

            if (rental.DepositReturned)
                return Result.Failure("Deposit has already been returned.", ErrorCodes.Validation);

            rental.DepositReturned = true;
            await _rentalRepository.UpdateAsync(rental);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(
                $"An error occurred while returning deposit: {ex.Message}", 
                ErrorCodes.ServerError);
        }
    }

    // Helper method to calculate subtotal based on rental period
    private decimal CalculateSubtotal(decimal rentalRate, DateTime startDate, DateTime endDate, string periodType)
    {
        var duration = (endDate - startDate).TotalDays;
        
        return periodType.ToLower() switch
        {
            "hourly" => rentalRate * (decimal)(endDate - startDate).TotalHours,
            "daily" => rentalRate * (decimal)Math.Ceiling(duration),
            "weekly" => rentalRate * (decimal)Math.Ceiling(duration / 7),
            "monthly" => rentalRate * (decimal)Math.Ceiling(duration / 30),
            _ => rentalRate * (decimal)Math.Ceiling(duration)
        };
    }
}

