using Firmness.Application.DTOs;

namespace Firmness.Application.Interfaces;
public interface IDashboardService
{
    Task<DashboardDto> GetDashboardDataAsync(CancellationToken cancellationToken = default);
    Task<int> GetTotalProductsAsync(CancellationToken cancellationToken = default);
    Task<int> GetTotalCustomersAsync(CancellationToken cancellationToken = default);
    Task<int> GetTotalSalesAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken = default);
}

