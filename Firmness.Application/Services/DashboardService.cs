using Firmness.Application.Interfaces;
using Firmness.Domain.DTOs;
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
        var totalProductsTask = GetTotalProductsAsync(cancellationToken);
        var totalCustomersTask = GetTotalCustomersAsync(cancellationToken);
        var totalSalesTask = GetTotalSalesAsync(cancellationToken);
        var totalRevenueTask = GetTotalRevenueAsync(cancellationToken);
        var recentSalesTask = GetRecentSalesAsync(cancellationToken);
        var lowStockProductsTask = GetLowStockProductsAsync(cancellationToken);
        var topCustomersTask = GetTopCustomersAsync(cancellationToken);
        var pendingSalesTask = GetPendingSalesCountAsync(cancellationToken);
        var completedSalesTask = GetCompletedSalesCountAsync(cancellationToken);

        await Task.WhenAll(
            totalProductsTask,
            totalCustomersTask,
            totalSalesTask,
            totalRevenueTask,
            recentSalesTask,
            lowStockProductsTask,
            topCustomersTask,
            pendingSalesTask,
            completedSalesTask
        );

        return new DashboardDto
        {
            TotalProducts = await totalProductsTask,
            TotalCustomers = await totalCustomersTask,
            TotalSales = await totalSalesTask,
            TotalRevenue = await totalRevenueTask,
            RecentSales = await recentSalesTask,
            LowStockProducts = await lowStockProductsTask,
            TopCustomers = await topCustomersTask,
            PendingSales = await pendingSalesTask,
            CompletedSales = await completedSalesTask
        };
    }


    public async Task<int> GetTotalProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepo.GetAllAsync();
        return products.Count();
    }

    public async Task<int> GetTotalCustomersAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _customerRepo.GetAllAsync();
        return customers.Count();
    }

    public async Task<int> GetTotalSalesAsync(CancellationToken cancellationToken = default)
    {
        var sales = await _saleRepo.GetAllAsync();
        return sales.Count();
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

