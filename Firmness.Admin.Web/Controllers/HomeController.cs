using System.Diagnostics;
using Firmness.Web.Models;
using Firmness.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var dashboardData = new DashboardViewModel
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalCustomers = await _context.Customers.CountAsync(),
                TotalSales = await _context.Sales.CountAsync(),
                TotalRevenue = await _context.Sales.SumAsync(s => s.TotalAmount),
                PendingSales = await _context.Sales.CountAsync(s => s.Status == Firmness.Domain.Enums.SaleStatus.Pending),
                CompletedSales = await _context.Sales.CountAsync(s => s.Status == Firmness.Domain.Enums.SaleStatus.Completed),
                RecentSales = await _context.Sales
                    .Include(s => s.Customer)
                    .OrderByDescending(s => s.CreatedAt)
                    .Take(5)
                    .ToListAsync(),
                LowStockProducts = await _context.Products
                    .Where(p => p.Stock <= 10)
                    .OrderBy(p => p.Stock)
                    .Take(5)
                    .ToListAsync(),
                TopCustomers = await _context.Customers
                    .Include(c => c.Sales)
                    .OrderByDescending(c => c.Sales.Count)
                    .Take(5)
                    .ToListAsync()
            };

            return View(dashboardData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard data");
            return View(new DashboardViewModel());
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}