using Firmness.Domain.Entities;
using Firmness.Domain.Enums;

namespace Firmness.Domain.Interfaces;

// Interface for vehicle rental repository
public interface IVehicleRentalRepository
{
    // CRUD operations
    Task<VehicleRental?> GetByIdAsync(Guid id);
    Task<VehicleRental?> GetByIdWithDetailsAsync(Guid id); // Include Customer and Vehicle
    Task<IEnumerable<VehicleRental>> GetAllAsync();
    Task<IEnumerable<VehicleRental>> GetAllWithDetailsAsync(); // Include Customer and Vehicle
    Task<IEnumerable<VehicleRental>> GetByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<VehicleRental>> GetByVehicleIdAsync(Guid vehicleId);
    Task<IEnumerable<VehicleRental>> GetByStatusAsync(RentalStatus status);
    Task AddAsync(VehicleRental rental);
    Task UpdateAsync(VehicleRental rental);
    Task DeleteAsync(Guid id);
    
    // Paged operations
    Task<(IEnumerable<VehicleRental> Items, long Total)> GetPagedAsync(
        int page, 
        int pageSize, 
        Guid? customerId = null,
        Guid? vehicleId = null,
        RentalStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null);
    
    Task<(IEnumerable<VehicleRental> Items, long Total)> GetPagedWithDetailsAsync(
        int page, 
        int pageSize, 
        Guid? customerId = null,
        Guid? vehicleId = null,
        RentalStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null);
    
    // Availability operations
    Task<bool> IsVehicleAvailableAsync(Guid vehicleId, DateTime startDate, DateTime endDate, Guid? excludeRentalId = null);
    Task<IEnumerable<VehicleRental>> GetConflictingRentalsAsync(Guid vehicleId, DateTime startDate, DateTime endDate, Guid? excludeRentalId = null);
    
    // Active rentals
    Task<IEnumerable<VehicleRental>> GetActiveRentalsAsync();
    Task<IEnumerable<VehicleRental>> GetActiveRentalsByVehicleIdAsync(Guid vehicleId);
    Task<VehicleRental?> GetActiveRentalByVehicleIdAsync(Guid vehicleId);
    
    // Overdue rentals
    Task<IEnumerable<VehicleRental>> GetOverdueRentalsAsync();
    Task<IEnumerable<VehicleRental>> GetOverdueRentalsWithDetailsAsync();
    
    // Upcoming returns
    Task<IEnumerable<VehicleRental>> GetUpcomingReturnsAsync(int days = 7);
    Task<IEnumerable<VehicleRental>> GetUpcomingReturnsWithDetailsAsync(int days = 7);
    
    // Rentals by date range
    Task<IEnumerable<VehicleRental>> GetRentalsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<VehicleRental>> GetRentalsByDateRangeWithDetailsAsync(DateTime startDate, DateTime endDate);
    
    // Statistics
    Task<long> CountAsync(CancellationToken cancellationToken = default);
    Task<long> CountByStatusAsync(RentalStatus status, CancellationToken cancellationToken = default);
    Task<long> CountActiveAsync(CancellationToken cancellationToken = default);
    Task<long> CountOverdueAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetTotalRevenueByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<decimal> GetPendingPaymentsAsync(CancellationToken cancellationToken = default);
    
    // Statistics by vehicle
    Task<int> GetTotalRentalsByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalRevenueByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken = default);
    Task<double> GetAverageRentalDurationByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken = default);
}

