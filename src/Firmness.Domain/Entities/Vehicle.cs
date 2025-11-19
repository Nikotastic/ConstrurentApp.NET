using Firmness.Domain.Enums;

namespace Firmness.Domain.Entities;

public class Vehicle : BaseEntity
{
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public VehicleType VehicleType { get; set; }
    
    // Rental information
    public decimal HourlyRate { get; set; } // Hourly rate
    public decimal DailyRate { get; set; } // Day rate
    public decimal WeeklyRate { get; set; } // Week rate
    public decimal MonthlyRate { get; set; } // Monthly rate
    
    // Status and availability
    public VehicleStatus Status { get; set; } = VehicleStatus.Available;
    public bool IsActive { get; set; } = true;
    
    // Technical Information
    public decimal? CurrentHours { get; set; } // Current usage hours
    public decimal? CurrentMileage { get; set; } // Current mileage
    public string? Specifications { get; set; } // JSON with technical specifications
    public string? SerialNumber { get; set; }
    
    // Maintenance
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public decimal? MaintenanceHoursInterval { get; set; }// Maintenance interval in hours
    
    // Multimedia
    public string? ImageUrl { get; set; }
    public string? DocumentsUrl { get; set; } // URL of documents (secure, etc.)
    
    public string? Notes { get; set; }
    
    // Navigation properties
    public ICollection<VehicleRental> Rentals { get; set; } = new List<VehicleRental>();
    
    protected Vehicle() { }
    
    public Vehicle(string brand, string model, int year, string licensePlate, VehicleType vehicleType)
    {
        Brand = brand;
        Model = model;
        Year = year;
        LicensePlate = licensePlate;
        VehicleType = vehicleType;
        Status = VehicleStatus.Available;
        IsActive = true;
    }
    
    // Full vehicle name for display
 
    public string DisplayName => $"{Brand} {Model} ({Year}) - {LicensePlate}";
    
    // Checks if the vehicle is available for rent
    public bool IsAvailableForRent => IsActive && Status == VehicleStatus.Available;
    
    // Checks if the vehicle needs maintenance soon
    public bool NeedsMaintenance
    {
        get
        {
            if (NextMaintenanceDate.HasValue)
            {
                return NextMaintenanceDate.Value <= DateTime.UtcNow.AddDays(7);
            }
            return false;
        }
    }
}

