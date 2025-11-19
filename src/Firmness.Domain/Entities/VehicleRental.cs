﻿using Firmness.Domain.Enums;

namespace Firmness.Domain.Entities;

public class VehicleRental : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Guid VehicleId { get; set; }
    // Dates
    public DateTime StartDate { get; set; }
    public DateTime EstimatedReturnDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    
    // Rental information
    public RentalStatus Status { get; set; } = RentalStatus.Pending;
    public decimal RentalRate { get; set; } // Agreed rate (can be hourly, daily, weekly, etc.)
    public string RentalPeriodType { get; set; } = "Daily"; // Hourly, Daily, Weekly, Monthly
    public decimal Deposit { get; set; } // Security deposit
    
    // Amounts
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; } = 0;
    public decimal PendingAmount { get; set; }
    
    // Pago
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public bool DepositReturned { get; set; } = false;
    
    // Locations
    public string? PickupLocation { get; set; }
    public string? ReturnLocation { get; set; }
    
    // Documentation
    public string? ContractUrl { get; set; } 
    public string? InvoiceNumber { get; set; }
    
    // Inspection
    public decimal? InitialHours { get; set; } 
    public decimal? FinalHours { get; set; } 
    public decimal? InitialMileage { get; set; }
    public decimal? FinalMileage { get; set; }
    public string? InitialCondition { get; set; } 
    public string? FinalCondition { get; set; } 
    
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    
    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public Vehicle Vehicle { get; set; } = null!;
    
    protected VehicleRental() { }
    
    public VehicleRental(Guid customerId, Guid vehicleId, DateTime startDate, DateTime estimatedReturnDate, decimal rentalRate)
    {
        CustomerId = customerId;
        VehicleId = vehicleId;
        StartDate = startDate;
        EstimatedReturnDate = estimatedReturnDate;
        RentalRate = rentalRate;
        Status = RentalStatus.Pending;
    }
    
    // Calculates the rental duration in days
    public int DurationInDays
    {
        get
        {
            var endDate = ActualReturnDate ?? EstimatedReturnDate;
            return (endDate - StartDate).Days;
        }
    }
    

    // Checks if the rental is overdue
    public bool IsOverdue
    {
        get
        {
            if (Status == RentalStatus.Active && !ActualReturnDate.HasValue)
            {
                return DateTime.UtcNow > EstimatedReturnDate;
            }
            return false;
        }
    }
    
    // Calculates the pending payment amount
    public void CalculatePendingAmount()
    {
        PendingAmount = TotalAmount - PaidAmount;
    }
    
    // Completes the rental
    public void CompleteRental(DateTime returnDate)
    {
        ActualReturnDate = returnDate;
        Status = RentalStatus.Completed;
    }
    
    // Cancels the rental

    public void CancelRental(string reason)
    {
        Status = RentalStatus.Cancelled;
        CancellationReason = reason;
    }
}


