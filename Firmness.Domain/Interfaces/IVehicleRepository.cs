using Firmness.Domain.Entities;
using Firmness.Domain.Enums;

namespace Firmness.Domain.Interfaces;

// Interface for vehicle repository
public interface IVehicleRepository
{
    // CRUD operations
    Task<Vehicle?> GetByIdAsync(Guid id);
    Task<IEnumerable<Vehicle>> GetAllAsync();
    Task<IEnumerable<Vehicle>> GetAllActiveAsync();
    Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync();
    Task<Vehicle?> GetByLicensePlateAsync(string licensePlate);
    Task<IEnumerable<Vehicle>> GetByTypeAsync(VehicleType vehicleType);
    Task<IEnumerable<Vehicle>> GetByStatusAsync(VehicleStatus status);
    Task<IEnumerable<Vehicle>> SearchAsync(string? query);
    Task AddAsync(Vehicle vehicle);
    Task UpdateAsync(Vehicle vehicle);
    Task DeleteAsync(Guid id);
    
    // Paged operations
    Task<(IEnumerable<Vehicle> Items, long Total)> GetPagedAsync(int page, int pageSize, string? query, VehicleType? vehicleType, VehicleStatus? status, bool? isActive);
    
    // Maintenance operations
    Task<IEnumerable<Vehicle>> GetVehiclesNeedingMaintenanceAsync();
    Task<IEnumerable<Vehicle>> GetVehiclesByMaintenanceDateRangeAsync(DateTime startDate, DateTime endDate);
    
    // Statistics
    Task<long> CountAsync(CancellationToken cancellationToken);
    Task<long> CountByStatusAsync(VehicleStatus status, CancellationToken cancellationToken);
    Task<long> CountAvailableAsync(CancellationToken cancellationToken);
}

