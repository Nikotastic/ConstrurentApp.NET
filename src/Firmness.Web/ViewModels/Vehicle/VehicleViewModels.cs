using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Firmness.Web.ViewModels.Vehicle;

// ViewModel for vehicle list display
public class VehicleListViewModel
{
    public Guid Id { get; set; }
    public Guid PublicId { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public decimal DailyRate { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public bool IsAvailableForRent { get; set; }
    public bool NeedsMaintenance { get; set; }
    public decimal? CurrentHours { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
}

// ViewModel for paginated vehicle list
public class PaginatedVehicleListViewModel
{
    public List<VehicleListViewModel> Vehicles { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public string? Search { get; set; }
    public string? VehicleType { get; set; }
    public string? Status { get; set; }
    
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}

// ViewModel for creating/editing vehicles
public class VehicleFormViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Brand is required")]
    [StringLength(100, ErrorMessage = "Brand cannot exceed 100 characters")]
    [Display(Name = "Brand")]
    public string Brand { get; set; } = string.Empty;

    [Required(ErrorMessage = "Model is required")]
    [StringLength(100, ErrorMessage = "Model cannot exceed 100 characters")]
    [Display(Name = "Model")]
    public string Model { get; set; } = string.Empty;

    [Required(ErrorMessage = "Year is required")]
    [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
    [Display(Name = "Year")]
    public int Year { get; set; }

    [Required(ErrorMessage = "License Plate is required")]
    [StringLength(20, ErrorMessage = "License Plate cannot exceed 20 characters")]
    [Display(Name = "License Plate")]
    public string LicensePlate { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vehicle Type is required")]
    [Display(Name = "Vehicle Type")]
    public int VehicleType { get; set; }

    [Required(ErrorMessage = "Hourly Rate is required")]
    [Range(0, 999999.99, ErrorMessage = "Hourly Rate must be positive")]
    [Display(Name = "Hourly Rate")]
    public decimal HourlyRate { get; set; }

    [Required(ErrorMessage = "Daily Rate is required")]
    [Range(0, 999999.99, ErrorMessage = "Daily Rate must be positive")]
    [Display(Name = "Daily Rate")]
    public decimal DailyRate { get; set; }

    [Required(ErrorMessage = "Weekly Rate is required")]
    [Range(0, 999999.99, ErrorMessage = "Weekly Rate must be positive")]
    [Display(Name = "Weekly Rate")]
    public decimal WeeklyRate { get; set; }

    [Required(ErrorMessage = "Monthly Rate is required")]
    [Range(0, 999999.99, ErrorMessage = "Monthly Rate must be positive")]
    [Display(Name = "Monthly Rate")]
    public decimal MonthlyRate { get; set; }

    [Display(Name = "Current Hours")]
    [Range(0, 999999.99, ErrorMessage = "Hours must be positive")]
    public decimal? CurrentHours { get; set; }

    [Display(Name = "Current Mileage")]
    [Range(0, 999999.99, ErrorMessage = "Mileage must be positive")]
    public decimal? CurrentMileage { get; set; }

    [Display(Name = "Specifications")]
    [StringLength(2000)]
    public string? Specifications { get; set; }

    [Display(Name = "Serial Number")]
    [StringLength(100)]
    public string? SerialNumber { get; set; }

    [Display(Name = "Maintenance Hours Interval")]
    [Range(0, 999999.99, ErrorMessage = "Must be positive")]
    public decimal? MaintenanceHoursInterval { get; set; }

    [Display(Name = "Last Maintenance Date")]
    public DateTime? LastMaintenanceDate { get; set; }

    [Display(Name = "Next Maintenance Date")]
    public DateTime? NextMaintenanceDate { get; set; }

    [Display(Name = "Image URL")]
    [Url(ErrorMessage = "Please enter a valid URL")]
    public string? ImageUrl { get; set; }

    [Display(Name = "Documents URL")]
    [Url(ErrorMessage = "Please enter a valid URL")]
    public string? DocumentsUrl { get; set; }

    [Display(Name = "Notes")]
    [StringLength(1000)]
    public string? Notes { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    // For dropdowns
    public List<SelectListItem> VehicleTypes { get; set; } = new();
}

// ViewModel for vehicle details
public class VehicleDetailsViewModel
{
    public Guid Id { get; set; }
    public Guid PublicId { get; set; } 
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public decimal DailyRate { get; set; }
    public decimal WeeklyRate { get; set; }
    public decimal MonthlyRate { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public decimal? CurrentHours { get; set; }
    public decimal? CurrentMileage { get; set; }
    public string? Specifications { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public decimal? MaintenanceHoursInterval { get; set; }
    public string? ImageUrl { get; set; }
    public string? DocumentsUrl { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public bool IsAvailableForRent { get; set; }
    public bool NeedsMaintenance { get; set; }
    
    // Rental history
    public List<VehicleRentalSummary> RecentRentals { get; set; } = new();
    public int TotalRentals { get; set; }
    public int ActiveRentals { get; set; }
}

// Summary for rental in vehicle details
public class VehicleRentalSummary
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EstimatedReturnDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}

