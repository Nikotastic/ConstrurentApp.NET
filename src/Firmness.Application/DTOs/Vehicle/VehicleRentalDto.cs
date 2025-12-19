﻿using Firmness.Domain.Enums;

namespace Firmness.Application.DTOs.Vehicle;

// DTO for reading vehicle rentals - API
public class VehicleRentalDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public Guid VehicleId { get; set; }
    public string VehicleDisplayName { get; set; } = string.Empty;
    public string VehicleBrand { get; set; } = string.Empty;
    public string VehicleModel { get; set; } = string.Empty;
    public string VehicleLicensePlate { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EstimatedReturnDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal RentalRate { get; set; }
    public string RentalPeriodType { get; set; } = string.Empty;
    public decimal Deposit { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal PendingAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public bool DepositReturned { get; set; }
    public string? PickupLocation { get; set; }
    public string? ReturnLocation { get; set; }
    public string? ContractUrl { get; set; }
    public string? InvoiceNumber { get; set; }
    public decimal? InitialHours { get; set; }
    public decimal? FinalHours { get; set; }
    public decimal? InitialMileage { get; set; }
    public decimal? FinalMileage { get; set; }
    public string? InitialCondition { get; set; }
    public string? FinalCondition { get; set; }
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int DurationInDays { get; set; }
    public bool IsOverdue { get; set; }
}

// DTO for creating vehicle rentals - Admin only via API
public class CreateVehicleRentalDto
{
    public Guid CustomerId { get; set; }
    public Guid VehicleId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EstimatedReturnDate { get; set; }
    public decimal RentalRate { get; set; }
    public string RentalPeriodType { get; set; } = "Daily";
    public decimal Deposit { get; set; }
    public decimal Discount { get; set; } = 0;
    public decimal Tax { get; set; } = 0;
    public PaymentMethod PaymentMethod { get; set; }
    public string? PickupLocation { get; set; }
    public string? ReturnLocation { get; set; }
    public decimal? InitialHours { get; set; }
    public decimal? InitialMileage { get; set; }
    public string? InitialCondition { get; set; }
    public string? Notes { get; set; }
}

// DTO for updating vehicle rentals - Admin only via API
public class UpdateVehicleRentalDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EstimatedReturnDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public RentalStatus? Status { get; set; }
    public decimal? RentalRate { get; set; }
    public string? RentalPeriodType { get; set; }
    public decimal? Deposit { get; set; }
    public decimal? Discount { get; set; }
    public decimal? Tax { get; set; }
    public decimal? PaidAmount { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public bool? DepositReturned { get; set; }
    public string? PickupLocation { get; set; }
    public string? ReturnLocation { get; set; }
    public string? ContractUrl { get; set; }
    public string? InvoiceNumber { get; set; }
    public decimal? FinalHours { get; set; }
    public decimal? FinalMileage { get; set; }
    public string? FinalCondition { get; set; }
    public string? Notes { get; set; }
}

// DTO for rental list - simplified for lists
public class VehicleRentalListDto
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string VehicleDisplayName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EstimatedReturnDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PendingAmount { get; set; }
    public bool IsOverdue { get; set; }
}

// DTO for completing a rental
public class CompleteVehicleRentalDto
{
    public DateTime ReturnDate { get; set; }
    public decimal? FinalHours { get; set; }
    public decimal? FinalMileage { get; set; }
    public string? FinalCondition { get; set; }
    public bool DepositReturned { get; set; }
    public string? Notes { get; set; }
}

// DTO for canceling a rental
public class CancelVehicleRentalDto
{
    public string CancellationReason { get; set; } = string.Empty;
    public bool RefundDeposit { get; set; }
}

