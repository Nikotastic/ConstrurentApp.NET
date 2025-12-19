using Firmness.Domain.Enums;

namespace Firmness.Application.DTOs.Vehicle;

// DTO for reading vehicles - API
public class VehicleDto
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
}

// DTO for creating vehicles - Admin only via API
public class CreateVehicleDto
{
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public VehicleType VehicleType { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal DailyRate { get; set; }
    public decimal WeeklyRate { get; set; }
    public decimal MonthlyRate { get; set; }
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
}

// DTO for updating vehicles - Admin only via API
public class UpdateVehicleDto
{
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public string? LicensePlate { get; set; }
    public VehicleType? VehicleType { get; set; }
    public decimal? HourlyRate { get; set; }
    public decimal? DailyRate { get; set; }
    public decimal? WeeklyRate { get; set; }
    public decimal? MonthlyRate { get; set; }
    public VehicleStatus? Status { get; set; }
    public bool? IsActive { get; set; }
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
}

// DTO for vehicle list - simplified for lists
public class VehicleListDto
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
    public string? ImageUrl { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public bool IsAvailableForRent { get; set; }
}

