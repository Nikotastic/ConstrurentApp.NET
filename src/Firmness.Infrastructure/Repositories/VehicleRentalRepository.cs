using Firmness.Domain.Entities;
using Firmness.Domain.Enums;
using Firmness.Domain.Interfaces;
using Firmness.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Infrastructure.Repositories;

// Implementation of IVehicleRentalRepository
public class VehicleRentalRepository : IVehicleRentalRepository
{
    private readonly ApplicationDbContext _db;
    
    public VehicleRentalRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    
    // CRUD operations
    public async Task<VehicleRental?> GetByIdAsync(Guid id)
    {
        return await _db.VehicleRentals
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);
    }
    
    public async Task<VehicleRental?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _db.VehicleRentals
            .AsNoTracking()
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
    
    public async Task<IEnumerable<VehicleRental>> GetAllAsync()
    {
        return await _db.VehicleRentals
            .AsNoTracking()
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<VehicleRental>> GetAllWithDetailsAsync()
    {
        return await _db.VehicleRentals
            .AsNoTracking()
            .AsSplitQuery() // Fix concurrency issue
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<VehicleRental>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _db.VehicleRentals
            .AsNoTracking()
            .Include(r => r.Vehicle)
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<VehicleRental>> GetByVehicleIdAsync(Guid vehicleId)
    {
        return await _db.VehicleRentals
            .AsNoTracking()
            .Include(r => r.Vehicle)
            .Where(r => r.Vehicle.Id == vehicleId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<VehicleRental>> GetByStatusAsync(RentalStatus status)
    {
        return await _db.VehicleRentals
            .AsNoTracking()
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
    
    public async Task AddAsync(VehicleRental rental)
    {
        if (rental == null) throw new ArgumentNullException(nameof(rental));
        await _db.VehicleRentals.AddAsync(rental);
    }
    
    public Task UpdateAsync(VehicleRental rental)
    {
        if (rental == null) throw new ArgumentNullException(nameof(rental));
        _db.VehicleRentals.Update(rental);
        return Task.CompletedTask;
    }
    
    public async Task DeleteAsync(Guid id)
    {
        var rental = await _db.VehicleRentals.FindAsync(id);
        if (rental != null)
        {
            _db.VehicleRentals.Remove(rental);
        }
    }
    
    // Paged operations
    public async Task<(IEnumerable<VehicleRental> Items, long Total)> GetPagedAsync(
        int page, 
        int pageSize, 
        Guid? customerId = null,
        Guid? vehicleId = null,
        RentalStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        
        var q = _db.VehicleRentals.AsNoTracking().AsQueryable();
        
        // Apply filters
        if (customerId.HasValue)
        {
            q = q.Where(r => r.CustomerId == customerId.Value);
        }
        
        if (vehicleId.HasValue)
        {
            q = q.Where(r => r.VehicleId == vehicleId.Value);
        }
        
        if (status.HasValue)
        {
            q = q.Where(r => r.Status == status.Value);
        }
        
        if (startDate.HasValue)
        {
            q = q.Where(r => r.StartDate >= startDate.Value);
        }
        
        if (endDate.HasValue)
        {
            q = q.Where(r => r.StartDate <= endDate.Value);
        }
        
        var total = await q.LongCountAsync();
        var items = await q
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (items, total);
    }
    
    public async Task<(IEnumerable<VehicleRental> Items, long Total)> GetPagedWithDetailsAsync(
        int page, 
        int pageSize, 
        Guid? customerId = null,
        Guid? vehicleId = null,
        RentalStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        
        var q = _db.VehicleRentals
            .AsNoTracking()
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .AsQueryable();
        
        // Apply filters
        if (customerId.HasValue)
        {
            q = q.Where(r => r.CustomerId == customerId.Value);
        }
        
        if (vehicleId.HasValue)
        {
            q = q.Where(r => r.Vehicle.Id == vehicleId.Value);
        }
        
        if (status.HasValue)
        {
            q = q.Where(r => r.Status == status.Value);
        }
        
        if (startDate.HasValue)
        {
            q = q.Where(r => r.StartDate >= startDate.Value);
        }
        
        if (endDate.HasValue)
        {
            q = q.Where(r => r.StartDate <= endDate.Value);
        }
        
        var total = await q.LongCountAsync();
        var items = await q
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (items, total);
    }
    
    // Availability operations
    public async Task<bool> IsVehicleAvailableAsync(
        Guid vehicleId, 
        DateTime startDate, 
        DateTime endDate, 
        Guid? excludeRentalId = null)
    {
        var conflictingRentals = await GetConflictingRentalsAsync(vehicleId, startDate, endDate, excludeRentalId);
        return !conflictingRentals.Any();
    }
    
    public async Task<IEnumerable<VehicleRental>> GetConflictingRentalsAsync(
        Guid vehicleId, 
        DateTime startDate, 
        DateTime endDate, 
        Guid? excludeRentalId = null)
    {
        var query = _db.VehicleRentals
            .AsNoTracking()
            .Include(r => r.Vehicle)
            .Where(r => r.Vehicle.Id == vehicleId &&
                   (r.Status == RentalStatus.Pending || r.Status == RentalStatus.Active) &&
                   r.StartDate < endDate &&
                   r.EstimatedReturnDate > startDate);
        
        if (excludeRentalId.HasValue)
        {
            query = query.Where(r => r.Id != excludeRentalId.Value);
        }
        
        return await query.ToListAsync();
    }
    
    // Active rentals
    public async Task<IEnumerable<VehicleRental>> GetActiveRentalsAsync()
    {
        return await _db.VehicleRentals
            .AsNoTracking()
            .Where(r => r.Status == RentalStatus.Active)
            .OrderBy(r => r.EstimatedReturnDate)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<VehicleRental>> GetActiveRentalsByVehicleIdAsync(Guid vehicleId)
    {
        return await _db.VehicleRentals
            .AsNoTracking()
            .Include(r => r.Vehicle)
            .Where(r => r.Vehicle.Id == vehicleId && r.Status == RentalStatus.Active)
            .OrderBy(r => r.EstimatedReturnDate)
            .ToListAsync();
    }
    
    public async Task<VehicleRental?> GetActiveRentalByVehicleIdAsync(Guid vehicleId)
    {
        return await _db.VehicleRentals
            .AsNoTracking()
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .FirstOrDefaultAsync(r => r.Vehicle.Id == vehicleId && r.Status == RentalStatus.Active);
    }
    
    // Overdue rentals
    public async Task<IEnumerable<VehicleRental>> GetOverdueRentalsAsync()
    {
        var now = DateTime.UtcNow;
        return await _db.VehicleRentals
            .AsNoTracking()
            .Where(r => r.Status == RentalStatus.Active && 
                   !r.ActualReturnDate.HasValue && 
                   r.EstimatedReturnDate < now)
            .OrderBy(r => r.EstimatedReturnDate)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<VehicleRental>> GetOverdueRentalsWithDetailsAsync()
    {
        var now = DateTime.UtcNow;
        return await _db.VehicleRentals
            .AsNoTracking()
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => r.Status == RentalStatus.Active && 
                   !r.ActualReturnDate.HasValue && 
                   r.EstimatedReturnDate < now)
            .OrderBy(r => r.EstimatedReturnDate)
            .ToListAsync();
    }
    
    // Upcoming returns
    public async Task<IEnumerable<VehicleRental>> GetUpcomingReturnsAsync(int days = 7)
    {
        var now = DateTime.UtcNow;
        var futureDate = now.AddDays(days);
        
        return await _db.VehicleRentals
            .AsNoTracking()
            .Where(r => r.Status == RentalStatus.Active && 
                   !r.ActualReturnDate.HasValue && 
                   r.EstimatedReturnDate >= now && 
                   r.EstimatedReturnDate <= futureDate)
            .OrderBy(r => r.EstimatedReturnDate)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<VehicleRental>> GetUpcomingReturnsWithDetailsAsync(int days = 7)
    {
        var now = DateTime.UtcNow;
        var futureDate = now.AddDays(days);
        
        return await _db.VehicleRentals
            .AsNoTracking()
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => r.Status == RentalStatus.Active && 
                   !r.ActualReturnDate.HasValue && 
                   r.EstimatedReturnDate >= now && 
                   r.EstimatedReturnDate <= futureDate)
            .OrderBy(r => r.EstimatedReturnDate)
            .ToListAsync();
    }
    
    // Rentals by date range
    public async Task<IEnumerable<VehicleRental>> GetRentalsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _db.VehicleRentals
            .AsNoTracking()
            .Where(r => r.StartDate >= startDate && r.StartDate <= endDate)
            .OrderByDescending(r => r.StartDate)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<VehicleRental>> GetRentalsByDateRangeWithDetailsAsync(DateTime startDate, DateTime endDate)
    {
        return await _db.VehicleRentals
            .AsNoTracking()
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => r.StartDate >= startDate && r.StartDate <= endDate)
            .OrderByDescending(r => r.StartDate)
            .ToListAsync();
    }
    
    // Statistics
    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _db.VehicleRentals.LongCountAsync(cancellationToken);
    }
    
    public async Task<long> CountByStatusAsync(RentalStatus status, CancellationToken cancellationToken = default)
    {
        return await _db.VehicleRentals
            .Where(r => r.Status == status)
            .LongCountAsync(cancellationToken);
    }
    
    public async Task<long> CountActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _db.VehicleRentals
            .Where(r => r.Status == RentalStatus.Active)
            .LongCountAsync(cancellationToken);
    }
    
    public async Task<long> CountOverdueAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _db.VehicleRentals
            .Where(r => r.Status == RentalStatus.Active && 
                   !r.ActualReturnDate.HasValue && 
                   r.EstimatedReturnDate < now)
            .LongCountAsync(cancellationToken);
    }
    
    public async Task<decimal> GetTotalRevenueByDateRangeAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        return await _db.VehicleRentals
            .Where(r => r.Status == RentalStatus.Completed && 
                   r.StartDate >= startDate && 
                   r.StartDate <= endDate)
            .SumAsync(r => r.TotalAmount, cancellationToken);
    }
    
    public async Task<decimal> GetPendingPaymentsAsync(CancellationToken cancellationToken = default)
    {
        return await _db.VehicleRentals
            .Where(r => r.Status == RentalStatus.Active || r.Status == RentalStatus.Pending)
            .SumAsync(r => r.PendingAmount, cancellationToken);
    }
    
    // Statistics by vehicle
    public async Task<int> GetTotalRentalsByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken = default)
    {
        return await _db.VehicleRentals
            .Where(r => r.VehicleId == vehicleId)
            .CountAsync(cancellationToken);
    }
    
    public async Task<decimal> GetTotalRevenueByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken = default)
    {
        return await _db.VehicleRentals
            .Where(r => r.VehicleId == vehicleId && r.Status == RentalStatus.Completed)
            .SumAsync(r => r.TotalAmount, cancellationToken);
    }
    
    public async Task<double> GetAverageRentalDurationByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken = default)
    {
        var rentals = await _db.VehicleRentals
            .Where(r => r.VehicleId == vehicleId && r.ActualReturnDate.HasValue)
            .Select(r => new { r.StartDate, r.ActualReturnDate })
            .ToListAsync(cancellationToken);
        
        if (!rentals.Any())
            return 0;
        
        var totalDays = rentals.Sum(r => (r.ActualReturnDate!.Value - r.StartDate).TotalDays);
        return totalDays / rentals.Count;
    }
}

