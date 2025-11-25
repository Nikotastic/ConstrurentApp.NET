using System.Diagnostics;
using Firmness.Web.Models;
using Firmness.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Firmness.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDashboardService _dashboardService;

    public HomeController(ILogger<HomeController> logger, IDashboardService dashboardService)
    {
        _logger = logger;
        _dashboardService = dashboardService;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        // If user is not authenticated, show landing page with empty model
        if (User.Identity?.IsAuthenticated != true)
        {
            _logger.LogInformation("User not authenticated - showing landing page");
            return View(new DashboardViewModel());
        }

        // If authenticated, load dashboard data
        _logger.LogInformation("User authenticated - loading dashboard data");
        try
        {
            var dashboardData = await _dashboardService.GetDashboardDataAsync();
            
            // Map DashboardDto to DashboardViewModel
            var viewModel = new DashboardViewModel
            {
                TotalProducts = dashboardData.TotalProducts,
                TotalCustomers = dashboardData.TotalCustomers,
                TotalSales = dashboardData.TotalSales,
                TotalRevenue = dashboardData.TotalRevenue,
                PendingSales = dashboardData.PendingSales,
                CompletedSales = dashboardData.CompletedSales,
                RecentSales = dashboardData.RecentSales,
                LowStockProducts = dashboardData.LowStockProducts,
                TopCustomers = dashboardData.TopCustomers
            };
            
            return View(viewModel);
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