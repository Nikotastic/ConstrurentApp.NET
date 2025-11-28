using Firmness.Application.Interfaces;
using Firmness.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Firmness.Web.Controllers;

[Authorize]
public class BulkImportController : Controller
{
    private readonly IBulkImportService _bulkImportService;
    private readonly IExcelTemplateService _excelTemplateService;
    private readonly ILogger<BulkImportController> _logger;

    public BulkImportController(
        IBulkImportService bulkImportService, 
        IExcelTemplateService excelTemplateService,
        ILogger<BulkImportController> logger)
    {
        _bulkImportService = bulkImportService;
        _excelTemplateService = excelTemplateService;
        _logger = logger;
    }

    // GET: BulkImport
    public IActionResult Index()
    {
        return View();
    }

    // POST: BulkImport/Upload
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Please select a valid Excel file";
            return RedirectToAction(nameof(Index));
        }

        // Validate extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension != ".xlsx" && extension != ".xls")
        {
            TempData["Error"] = "Only Excel files (.xlsx, .xls) are allowed";
            return RedirectToAction(nameof(Index));
        }

        // Validate size (max 10 MB)
        if (file.Length > 10 * 1024 * 1024)
        {
            TempData["Error"] = "File is too large. Maximum size: 10 MB";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            using var stream = file.OpenReadStream();
            var result = await _bulkImportService.ImportFromExcelAsync(stream);

            if (result.HasErrors)
            {
                TempData["Warning"] = $"Import completed with errors. {result.Summary}";
                return View("Result", result);
            }

            TempData["Success"] = $"Import successful! {result.Summary}";
            return View("Result", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing Excel file");
            TempData["Error"] = $"Error processing file: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: BulkImport/DownloadTemplate
    public IActionResult DownloadTemplate(string type = "products")
    {
        try
        {
            byte[] fileBytes;
            string fileName;

            switch (type.ToLower())
            {
                case "vehicles":
                    fileBytes = _excelTemplateService.GenerateVehicleTemplate();
                    fileName = $"Vehicle_Import_Template_{DateTime.Now:yyyyMMdd}.xlsx";
                    break;
                case "customers":
                    fileBytes = _excelTemplateService.GenerateCustomerTemplate();
                    fileName = $"Customer_Import_Template_{DateTime.Now:yyyyMMdd}.xlsx";
                    break;
                default:
                    fileBytes = _excelTemplateService.GenerateProductTemplate();
                    fileName = $"Product_Import_Template_{DateTime.Now:yyyyMMdd}.xlsx";
                    break;
            }
            
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating template");
            TempData["Error"] = "Error generating template";
            return RedirectToAction(nameof(Index));
        }
    }
    
    // GET: BulkImport/DownloadSample (deprecated - redirect to DownloadTemplate)
    public IActionResult DownloadSample()
    {
        return DownloadTemplate("products");
    }

    // GET: BulkImport/DownloadVehiclesSample (deprecated - redirect to DownloadTemplate)
    public IActionResult DownloadVehiclesSample()
    {
        return DownloadTemplate("vehicles");
    }

    // GET: BulkImport/DownloadSampleData
    public IActionResult DownloadSampleData()
    {
        try
        {
            var fileBytes = _excelTemplateService.GenerateSampleDataFile();
            var fileName = $"Sample_Data_{DateTime.Now:yyyyMMdd}.xlsx";
            
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating sample data");
            TempData["Error"] = "Error generating sample data";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: BulkImport/DownloadCategoriesReference
    public IActionResult DownloadCategoriesReference()
    {
        try
        {
            var fileBytes = _excelTemplateService.GenerateCategoriesReference();
            var fileName = $"Categories_Reference_{DateTime.Now:yyyyMMdd}.xlsx";
            
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating categories reference");
            TempData["Error"] = "Error generating categories reference";
            return RedirectToAction(nameof(Index));
        }
    }
}
