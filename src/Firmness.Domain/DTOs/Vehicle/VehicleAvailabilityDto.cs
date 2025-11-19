namespace Firmness.Domain.DTOs.Vehicle;

// DTO for checking vehicle availability
public class VehicleAvailabilityDto
{
    public Guid VehicleId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

// DTO for vehicle availability response
public class VehicleAvailabilityResponseDto
{
    public Guid VehicleId { get; set; }
    public string VehicleDisplayName { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public string? Reason { get; set; }
    public List<ConflictingRentalDto> ConflictingRentals { get; set; } = new();
}

// DTO for conflicting rentals
public class ConflictingRentalDto
{
    public Guid RentalId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EstimatedReturnDate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
}

// DTO for rental statistics
public class VehicleRentalStatsDto
{
    public Guid VehicleId { get; set; }
    public string VehicleDisplayName { get; set; } = string.Empty;
    public int TotalRentals { get; set; }
    public int ActiveRentals { get; set; }
    public int CompletedRentals { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageRentalDuration { get; set; }
    public DateTime? LastRentalDate { get; set; }
}

// DTO for dashboard rental summary
public class RentalDashboardDto
{
    public int TotalActiveRentals { get; set; }
    public int TotalPendingRentals { get; set; }
    public int TotalOverdueRentals { get; set; }
    public int AvailableVehicles { get; set; }
    public int RentedVehicles { get; set; }
    public int VehiclesInMaintenance { get; set; }
    public decimal TotalRevenueThisMonth { get; set; }
    public decimal PendingPayments { get; set; }
    public List<VehicleRentalListDto> RecentRentals { get; set; } = new();
    public List<VehicleRentalListDto> UpcomingReturns { get; set; } = new();
    public List<VehicleRentalListDto> OverdueRentals { get; set; } = new();
}

