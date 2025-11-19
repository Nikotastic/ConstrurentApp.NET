using Firmness.Domain.Entities;
using Firmness.Domain.Enums;
using Firmness.Domain.Interfaces;
using Firmness.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Infrastructure.Repositories;

// Implementation of IVehicleRepository
public class VehicleRepository : IVehicleRepository
{
    private readonly ApplicationDbContext _db;
    
    public VehicleRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    
    // CRUD operations
    public async Task<Vehicle?> GetByIdAsync(Guid id)
    {
        return await _db.Vehicles
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);
    }
    
    public async Task<IEnumerable<Vehicle>> GetAllAsync()
    {
        return await _db.Vehicles
            .AsNoTracking()
            .OrderBy(v => v.Brand)
            .ThenBy(v => v.Model)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Vehicle>> GetAllActiveAsync()
    {
        return await _db.Vehicles
            .AsNoTracking()
            .Where(v => v.IsActive)
            .OrderBy(v => v.Brand)
            .ThenBy(v => v.Model)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
    {
        return await _db.Vehicles
            .AsNoTracking()
            .Where(v => v.IsActive && v.Status == VehicleStatus.Available)
            .OrderBy(v => v.Brand)
            .ThenBy(v => v.Model)
            .ToListAsync();
    }
    
    public async Task<Vehicle?> GetByLicensePlateAsync(string licensePlate)
    {
        return await _db.Vehicles
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.LicensePlate == licensePlate);
    }
    
    public async Task<IEnumerable<Vehicle>> GetByTypeAsync(VehicleType vehicleType)
    {
        return await _db.Vehicles
            .AsNoTracking()
            .Where(v => v.VehicleType == vehicleType)
            .OrderBy(v => v.Brand)
            .ThenBy(v => v.Model)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Vehicle>> GetByStatusAsync(VehicleStatus status)
    {
        return await _db.Vehicles
            .AsNoTracking()
            .Where(v => v.Status == status)
            .OrderBy(v => v.Brand)
            .ThenBy(v => v.Model)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Vehicle>> SearchAsync(string? query)
    {
        var q = _db.Vehicles.AsNoTracking().AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(query))
        {
            q = q.Where(v => 
                v.Brand.Contains(query) || 
                v.Model.Contains(query) || 
                v.LicensePlate.Contains(query) ||
                v.SerialNumber != null && v.SerialNumber.Contains(query));
        }
        
        return await q
            .OrderBy(v => v.Brand)
            .ThenBy(v => v.Model)
            .ToListAsync();
    }
    
    public async Task AddAsync(Vehicle vehicle)
    {
        if (vehicle == null) throw new ArgumentNullException(nameof(vehicle));
        await _db.Vehicles.AddAsync(vehicle);
        await _db.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(Vehicle vehicle)
    {
        if (vehicle == null) throw new ArgumentNullException(nameof(vehicle));
        _db.Vehicles.Update(vehicle);
        await _db.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(Guid id)
    {
        var vehicle = await _db.Vehicles.FindAsync(id);
        if (vehicle != null)
        {
            _db.Vehicles.Remove(vehicle);
            await _db.SaveChangesAsync();
        }
    }
    
    // Paged operations
    public async Task<(IEnumerable<Vehicle> Items, long Total)> GetPagedAsync(
        int page, 
        int pageSize, 
        string? query, 
        VehicleType? vehicleType, 
        VehicleStatus? status, 
        bool? isActive)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        
        var q = _db.Vehicles.AsNoTracking().AsQueryable();
        
        // Apply filters
        if (!string.IsNullOrWhiteSpace(query))
        {
            q = q.Where(v => 
                v.Brand.Contains(query) || 
                v.Model.Contains(query) || 
                v.LicensePlate.Contains(query));
        }
        
        if (vehicleType.HasValue)
        {
            q = q.Where(v => v.VehicleType == vehicleType.Value);
        }
        
        if (status.HasValue)
        {
            q = q.Where(v => v.Status == status.Value);
        }
        
        if (isActive.HasValue)
        {
            q = q.Where(v => v.IsActive == isActive.Value);
        }
        
        var total = await q.LongCountAsync();
        var items = await q
            .OrderBy(v => v.Brand)
            .ThenBy(v => v.Model)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (items, total);
    }
    
    // Maintenance operations
    public async Task<IEnumerable<Vehicle>> GetVehiclesNeedingMaintenanceAsync()
    {
        var sevenDaysFromNow = DateTime.UtcNow.AddDays(7);
        
        return await _db.Vehicles
            .AsNoTracking()
            .Where(v => v.IsActive && 
                   v.NextMaintenanceDate.HasValue && 
                   v.NextMaintenanceDate.Value <= sevenDaysFromNow)
            .OrderBy(v => v.NextMaintenanceDate)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Vehicle>> GetVehiclesByMaintenanceDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _db.Vehicles
            .AsNoTracking()
            .Where(v => v.NextMaintenanceDate.HasValue && 
                   v.NextMaintenanceDate.Value >= startDate && 
                   v.NextMaintenanceDate.Value <= endDate)
            .OrderBy(v => v.NextMaintenanceDate)
            .ToListAsync();
    }
    
    // Statistics
    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Vehicles.LongCountAsync(cancellationToken);
    }
    
    public async Task<long> CountByStatusAsync(VehicleStatus status, CancellationToken cancellationToken = default)
    {
        return await _db.Vehicles
            .Where(v => v.Status == status)
            .LongCountAsync(cancellationToken);
    }
    
    public async Task<long> CountAvailableAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Vehicles
            .Where(v => v.IsActive && v.Status == VehicleStatus.Available)
            .LongCountAsync(cancellationToken);
    }
}

