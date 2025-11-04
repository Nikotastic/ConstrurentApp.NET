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
        var vm = new DashboardViewModel
        {
            TotalProducts = await _productService.CountAsync(),
            TotalCustomers = await _customerService.CountAsync(),
            TotalSales = await _saleService.CountAsync()
        };

        return View(vm);
    }
    // ViewModel simple para el dashboard
    
}