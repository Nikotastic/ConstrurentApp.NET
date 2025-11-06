using Firmness.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmness.Admin.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IProductService _productService;
    private readonly ICustomerService _customerService;
    private readonly ISaleService _saleService;
    private readonly ILogger<AdminController> _logger;
    
    public AdminController(IProductService productService, ICustomerService customerService, ISaleService saleService, ILogger<AdminController> logger)
    {
        _productService = productService;
        _customerService = customerService;
        _saleService = saleService;
        _logger = logger;
    }
    
    public async Task<IActionResult> Index()
    {
        var prodRes = await _productService.CountAsync();
        var custRes = await _customerService.CountAsync();
        var saleRes = await _saleService.CountAsync();

        if (!prodRes.IsSuccess) _logger.LogWarning("Failed to get product count: {Message}", prodRes.ErrorMessage);
        if (!custRes.IsSuccess) _logger.LogWarning("Failed to get customer count: {Message}", custRes.ErrorMessage);
        if (!saleRes.IsSuccess) _logger.LogWarning("Failed to get sale count: {Message}", saleRes.ErrorMessage);

        var vm = new DashboardViewModel
        {
            TotalProducts = prodRes.IsSuccess ? prodRes.Value : 0L,
            TotalCustomers = custRes.IsSuccess ? custRes.Value : 0L,
            TotalSales = saleRes.IsSuccess ? saleRes.Value : 0L
        };

        return View(vm);
    }
    // ViewModel simple para el dashboard
    
}