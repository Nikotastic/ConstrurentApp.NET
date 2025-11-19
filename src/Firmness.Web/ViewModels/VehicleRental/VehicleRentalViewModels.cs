﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Firmness.Web.ViewModels.VehicleRental;

// ViewModel for rental list display
public class VehicleRentalListViewModel
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
    public string? PickupLocation { get; set; }
    public int DurationInDays { get; set; }
}

// ViewModel for paginated rental list
public class PaginatedVehicleRentalListViewModel
{
    public List<VehicleRentalListViewModel> Rentals { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public string? Status { get; set; }
    public string? Search { get; set; }
    
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}

// ViewModel for creating rentals
public class CreateRentalViewModel
{
    [Required(ErrorMessage = "Customer is required")]
    [Display(Name = "Customer")]
    public Guid CustomerId { get; set; }

    [Required(ErrorMessage = "Vehicle is required")]
    [Display(Name = "Vehicle")]
    public Guid VehicleId { get; set; }

    [Required(ErrorMessage = "Start Date is required")]
    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Estimated Return Date is required")]
    [Display(Name = "Estimated Return Date")]
    public DateTime EstimatedReturnDate { get; set; } = DateTime.Now.AddDays(7);

    [Required(ErrorMessage = "Rental Rate is required")]
    [Range(0, 999999.99, ErrorMessage = "Rate must be positive")]
    [Display(Name = "Rental Rate")]
    public decimal RentalRate { get; set; }

    [Required(ErrorMessage = "Rental Period Type is required")]
    [Display(Name = "Rental Period Type")]
    public string RentalPeriodType { get; set; } = "Daily";

    [Required(ErrorMessage = "Deposit is required")]
    [Range(0, 999999.99, ErrorMessage = "Deposit must be positive")]
    [Display(Name = "Security Deposit")]
    public decimal Deposit { get; set; }

    [Range(0, 999999.99, ErrorMessage = "Discount must be positive")]
    [Display(Name = "Discount")]
    public decimal Discount { get; set; } = 0;

    [Range(0, 999999.99, ErrorMessage = "Tax must be positive")]
    [Display(Name = "Tax")]
    public decimal Tax { get; set; } = 0;

    [Required(ErrorMessage = "Payment Method is required")]
    [Display(Name = "Payment Method")]
    public int PaymentMethod { get; set; }

    [Display(Name = "Pickup Location")]
    [StringLength(200)]
    public string? PickupLocation { get; set; }

    [Display(Name = "Return Location")]
    [StringLength(200)]
    public string? ReturnLocation { get; set; }

    [Display(Name = "Initial Hours")]
    [Range(0, 999999.99)]
    public decimal? InitialHours { get; set; }

    [Display(Name = "Initial Mileage")]
    [Range(0, 999999.99)]
    public decimal? InitialMileage { get; set; }

    [Display(Name = "Initial Condition")]
    [StringLength(1000)]
    public string? InitialCondition { get; set; }

    [Display(Name = "Notes")]
    [StringLength(1000)]
    public string? Notes { get; set; }

    // For dropdowns
    public List<SelectListItem> Customers { get; set; } = new();
    public List<SelectListItem> Vehicles { get; set; } = new();
    public List<SelectListItem> PeriodTypes { get; set; } = new();
    public List<SelectListItem> PaymentMethods { get; set; } = new();
    
    // Calculated fields (read-only)
    public decimal CalculatedSubtotal { get; set; }
    public decimal CalculatedTotal { get; set; }
}

// ViewModel for completing rentals
public class CompleteRentalViewModel
{
    public Guid RentalId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string VehicleDisplayName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EstimatedReturnDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal PendingAmount { get; set; }
    public decimal Deposit { get; set; }
    public decimal? InitialHours { get; set; }
    public decimal? InitialMileage { get; set; }

    [Required(ErrorMessage = "Return Date is required")]
    [Display(Name = "Actual Return Date")]
    public DateTime ReturnDate { get; set; } = DateTime.Now;

    [Display(Name = "Final Hours")]
    [Range(0, 999999.99)]
    public decimal? FinalHours { get; set; }

    [Display(Name = "Final Mileage")]
    [Range(0, 999999.99)]
    public decimal? FinalMileage { get; set; }

    [Required(ErrorMessage = "Final Condition is required")]
    [Display(Name = "Final Condition")]
    [StringLength(1000)]
    public string FinalCondition { get; set; } = string.Empty;

    [Display(Name = "Return Deposit?")]
    public bool DepositReturned { get; set; } = true;

    [Display(Name = "Notes")]
    [StringLength(1000)]
    public string? Notes { get; set; }
}

// ViewModel for rental details
public class VehicleRentalDetailsViewModel
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
    
    // Payment history
    public List<PaymentRecord> PaymentHistory { get; set; } = new();
}

// ViewModel for Create/Edit form (unified)
public class VehicleRentalFormViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Customer is required")]
    [Display(Name = "Customer")]
    public Guid CustomerId { get; set; }

    [Required(ErrorMessage = "Vehicle is required")]
    [Display(Name = "Vehicle")]
    public Guid VehicleId { get; set; }

    [Required(ErrorMessage = "Start Date is required")]
    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Estimated Return Date is required")]
    [Display(Name = "Estimated Return Date")]
    public DateTime EstimatedReturnDate { get; set; } = DateTime.Today.AddDays(7);

    [Required(ErrorMessage = "Rental Rate is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Rate must be greater than 0")]
    [Display(Name = "Rental Rate (per period)")]
    public decimal RentalRate { get; set; }

    [Required(ErrorMessage = "Rental Period Type is required")]
    [Display(Name = "Period Type")]
    public string RentalPeriodType { get; set; } = "Daily";

    [Required(ErrorMessage = "Deposit is required")]
    [Range(0, 999999.99, ErrorMessage = "Deposit must be positive")]
    [Display(Name = "Security Deposit")]
    public decimal Deposit { get; set; }

    [Display(Name = "Pickup Location")]
    [StringLength(200)]
    public string? PickupLocation { get; set; }

    [Display(Name = "Return Location")]
    [StringLength(200)]
    public string? ReturnLocation { get; set; }

    [Display(Name = "Initial Hours")]
    [Range(0, 999999.99)]
    public decimal? InitialHours { get; set; }

    [Display(Name = "Initial Mileage (km)")]
    [Range(0, 999999.99)]
    public decimal? InitialMileage { get; set; }

    [Display(Name = "Initial Condition Notes")]
    [StringLength(1000)]
    public string? InitialCondition { get; set; }

    [Display(Name = "Additional Notes")]
    [StringLength(1000)]
    public string? Notes { get; set; }

    // Dropdowns
    public List<SelectListItem> Customers { get; set; } = new();
    public List<SelectListItem> Vehicles { get; set; } = new();
    public List<SelectListItem> RentalPeriodTypes { get; set; } = new();
    public List<SelectListItem> PaymentMethods { get; set; } = new();
}

// Payment record for history
public class PaymentRecord
{
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

// ViewModel for processing payment
public class ProcessPaymentViewModel
{
    public Guid RentalId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string VehicleDisplayName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal PendingAmount { get; set; }

    [Required(ErrorMessage = "Payment Amount is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Amount must be greater than 0")]
    [Display(Name = "Payment Amount")]
    public decimal Amount { get; set; }

    [Display(Name = "Payment Notes")]
    [StringLength(500)]
    public string? Notes { get; set; }
}

// ViewModel for dashboard
public class RentalDashboardViewModel
{
    public int TotalActiveRentals { get; set; }
    public int TotalPendingRentals { get; set; }
    public int TotalOverdueRentals { get; set; }
    public int AvailableVehicles { get; set; }
    public int RentedVehicles { get; set; }
    public int VehiclesInMaintenance { get; set; }
    
    public decimal TotalRevenueThisMonth { get; set; }
    public decimal TotalPendingPayments { get; set; }
    
    public List<VehicleRentalListViewModel> RecentRentals { get; set; } = new();
    public List<VehicleRentalListViewModel> OverdueRentals { get; set; } = new();
    public List<VehicleRentalListViewModel> UpcomingReturns { get; set; } = new();
}

