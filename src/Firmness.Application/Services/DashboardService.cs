using Firmness.Application.Interfaces;
using Firmness.Application.DTOs;
using Firmness.Domain.Entities;
using Firmness.Domain.Enums;
using Firmness.Domain.Interfaces;

namespace Firmness.Application.Services;


// Service to obtain dashboard data

public class DashboardService : IDashboardService
{
    private readonly IProductRepository _productRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly ISaleRepository _saleRepo;

    public DashboardService(
        IProductRepository productRepo,
        ICustomerRepository customerRepo,
        ISaleRepository saleRepo)
    {
        _productRepo = productRepo ?? throw new ArgumentNullException(nameof(productRepo));
        _customerRepo = customerRepo ?? throw new ArgumentNullException(nameof(customerRepo));
        _saleRepo = saleRepo ?? throw new ArgumentNullException(nameof(saleRepo));
    }

    public async Task<DashboardDto> GetDashboardDataAsync(CancellationToken cancellationToken = default)
    {
        // Execute sequentially to avoid DbContext concurrency issues
        // DbContext is not thread-safe, so we can't run multiple queries in parallel
        
        var totalProducts = await GetTotalProductsAsync(cancellationToken);
        var totalCustomers = await GetTotalCustomersAsync(cancellationToken);
        var totalSales = await GetTotalSalesAsync(cancellationToken);
        var totalRevenue = await GetTotalRevenueAsync(cancellationToken);
        var recentSales = await GetRecentSalesAsync(cancellationToken);
        var lowStockProducts = await GetLowStockProductsAsync(cancellationToken);
        var topCustomers = await GetTopCustomersAsync(cancellationToken);
        var pendingSales = await GetPendingSalesCountAsync(cancellationToken);
        var completedSales = await GetCompletedSalesCountAsync(cancellationToken);

        return new DashboardDto
        {
            TotalProducts = totalProducts,
            TotalCustomers = totalCustomers,
            TotalSales = totalSales,
            TotalRevenue = totalRevenue,
            RecentSales = recentSales,
            LowStockProducts = lowStockProducts,
            TopCustomers = topCustomers,
            PendingSales = pendingSales,
            CompletedSales = completedSales
        };
    }


    public async Task<int> GetTotalProductsAsync(CancellationToken cancellationToken = default)
    {
        var count = await _productRepo.CountAsync(cancellationToken);
        return (int)count;
    }

    public async Task<int> GetTotalCustomersAsync(CancellationToken cancellationToken = default)
    {
        var count = await _customerRepo.CountAsync(cancellationToken);
        return (int)count;
    }

    public async Task<int> GetTotalSalesAsync(CancellationToken cancellationToken = default)
    {
        var count = await _saleRepo.CountAsync(cancellationToken);
        return (int)count;
    }

    public async Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken = default)
    {
        var sales = await _saleRepo.GetAllAsync();
        return sales.Sum(s => s.TotalAmount);
    }

    private async Task<List<Sale>> GetRecentSalesAsync(CancellationToken cancellationToken = default)
    {
        var sales = await _saleRepo.GetAllWithDetailsAsync();
        return sales
            .OrderByDescending(s => s.CreatedAt)
            .Take(5)
            .ToList();
    }

    private async Task<List<Product>> GetLowStockProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepo.GetAllAsync();
        return products
            .Where(p => p.Stock <= 10)
            .OrderBy(p => p.Stock)
            .Take(5)
            .ToList();
    }

    private async Task<List<Customer>> GetTopCustomersAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _customerRepo.GetAllAsync();
        return customers
            .OrderByDescending(c => c.Sales.Count)
            .Take(5)
            .ToList();
    }

    private async Task<int> GetPendingSalesCountAsync(CancellationToken cancellationToken = default)
    {
        var sales = await _saleRepo.GetAllAsync();
        return sales.Count(s => s.Status == SaleStatus.Pending);
    }

    private async Task<int> GetCompletedSalesCountAsync(CancellationToken cancellationToken = default)
    {
        var sales = await _saleRepo.GetAllAsync();
        return sales.Count(s => s.Status == SaleStatus.Completed);
    }
}

