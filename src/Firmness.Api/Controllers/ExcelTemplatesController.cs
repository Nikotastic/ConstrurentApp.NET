using Firmness.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmness.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ExcelTemplatesController : ControllerBase
{
    private readonly IExcelTemplateService _templateService;
    private readonly ILogger<ExcelTemplatesController> _logger;

    public ExcelTemplatesController(
        IExcelTemplateService templateService,
        ILogger<ExcelTemplatesController> logger)
    {
        _templateService = templateService;
        _logger = logger;
    }

    // Download Excel template for product bulk import

    [HttpGet("products")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public IActionResult DownloadProductTemplate()
    {
        try
        {
            var fileBytes = _templateService.GenerateProductTemplate();
            var fileName = $"Product_Import_Template_{DateTime.Now:yyyyMMdd}.xlsx";
            
            return File(fileBytes, 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating product template");
            return StatusCode(500, new { message = "Error generating template" });
        }
    }

    // Download Excel template for vehicle bulk import
    [HttpGet("vehicles")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public IActionResult DownloadVehicleTemplate()
    {
        try
        {
            var fileBytes = _templateService.GenerateVehicleTemplate();
            var fileName = $"Vehicle_Import_Template_{DateTime.Now:yyyyMMdd}.xlsx";
            
            return File(fileBytes, 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating vehicle template");
            return StatusCode(500, new { message = "Error generating template" });
        }
    }

    // Download Excel template for customer bulk import
    [HttpGet("customers")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public IActionResult DownloadCustomerTemplate()
    {
        try
        {
            var fileBytes = _templateService.GenerateCustomerTemplate();
            var fileName = $"Customer_Import_Template_{DateTime.Now:yyyyMMdd}.xlsx";
            
            return File(fileBytes, 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating customer template");
            return StatusCode(500, new { message = "Error generating template" });
        }
    }

    // Get information about available templates
    [HttpGet("info")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult GetTemplateInfo()
    {
        return Ok(new
        {
            isSuccess = true,
            templates = new[]
            {
                new
                {
                    type = "products",
                    name = "Products Template",
                    description = "Template for bulk importing products with SKU, name, price, stock, etc.",
                    downloadUrl = "/api/exceltemplates/products",
                    requiredFields = new[] { "Name", "Price" },
                    optionalFields = new[] { "SKU", "Description", "Stock", "Image URL", "Category" }
                },
                new
                {
                    type = "vehicles",
                    name = "Vehicles Template",
                    description = "Template for bulk importing construction vehicles and machinery",
                    downloadUrl = "/api/exceltemplates/vehicles",
                    requiredFields = new[] { "Brand", "Model", "Year", "License Plate", "Vehicle Type", "Daily Rate" },
                    optionalFields = new[] { "Weekly Rate", "Monthly Rate", "Serial Number", "Image URL", "Notes" }
                },
                new
                {
                    type = "customers",
                    name = "Customers Template",
                    description = "Template for bulk importing customer data",
                    downloadUrl = "/api/exceltemplates/customers",
                    requiredFields = new[] { "First Name", "Email" },
                    optionalFields = new[] { "Last Name", "Document", "Phone", "Address" }
                }
            }
        });
    }
}
