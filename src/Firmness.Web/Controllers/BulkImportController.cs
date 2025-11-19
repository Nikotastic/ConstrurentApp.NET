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
            TempData["Error"] = "Por favor seleccione un archivo Excel válido";
            return RedirectToAction(nameof(Index));
        }

        // Validar extensión
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension != ".xlsx" && extension != ".xls")
        {
            TempData["Error"] = "Solo se permiten archivos Excel (.xlsx, .xls)";
            return RedirectToAction(nameof(Index));
        }

        // Validar tamaño (máximo 10 MB)
        if (file.Length > 10 * 1024 * 1024)
        {
            TempData["Error"] = "El archivo es demasiado grande. Tamaño máximo: 10 MB";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            using var stream = file.OpenReadStream();
            var result = await _bulkImportService.ImportFromExcelAsync(stream);

            if (result.HasErrors)
            {
                TempData["Warning"] = $"Importación completada con errores. {result.Summary}";
                return View("Result", result);
            }

            TempData["Success"] = $"¡Importación exitosa! {result.Summary}";
            return View("Result", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al importar archivo Excel");
            TempData["Error"] = $"Error al procesar el archivo: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: BulkImport/DownloadTemplate
    public IActionResult DownloadTemplate()
    {
        try
        {
            var fileBytes = _excelTemplateService.GenerateTemplate();
            var fileName = "BulkImport_Template.xlsx";
            
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating template");
            TempData["Error"] = "Error al crear la plantilla";
            return RedirectToAction(nameof(Index));
        }
    }
    
    // GET: BulkImport/DownloadSample
    public IActionResult DownloadSample()
    {
        try
        {
            var fileBytes = _excelTemplateService.GenerateSampleData();
            var fileName = "BulkImport_SampleData.xlsx";
            
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating sample data");
            TempData["Error"] = "Error al crear el archivo de ejemplo";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: BulkImport/DownloadVehiclesSample
    public IActionResult DownloadVehiclesSample()
    {
        try
        {
            var fileBytes = _excelTemplateService.GenerateVehiclesSampleData();
            var fileName = $"Vehicles_BulkImport_Sample_{DateTime.Now:yyyyMMdd}.xlsx";
            
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating vehicles sample data");
            TempData["Error"] = "Error al crear el archivo de ejemplo de vehículos";
            return RedirectToAction(nameof(Index));
        }
    }
}
