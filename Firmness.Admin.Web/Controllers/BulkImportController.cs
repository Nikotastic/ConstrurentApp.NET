using Firmness.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmness.Admin.Web.Controllers;

[Authorize]
public class BulkImportController : Controller
{
    private readonly IBulkImportService _bulkImportService;
    private readonly ILogger<BulkImportController> _logger;

    public BulkImportController(IBulkImportService bulkImportService, ILogger<BulkImportController> logger)
    {
        _bulkImportService = bulkImportService;
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
        // Crear un archivo Excel de plantilla
        var fileName = "Plantilla_ImportacionMasiva.xlsx";
        var filePath = Path.Combine(Path.GetTempPath(), fileName);

        try
        {
            CreateTemplateFile(filePath);
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear plantilla");
            TempData["Error"] = "Error al crear la plantilla";
            return RedirectToAction(nameof(Index));
        }
    }

    private void CreateTemplateFile(string filePath)
    {
        using var package = new OfficeOpenXml.ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Datos");

        // Encabezados
        worksheet.Cells[1, 1].Value = "SKU";
        worksheet.Cells[1, 2].Value = "Producto";
        worksheet.Cells[1, 3].Value = "Descripcion";
        worksheet.Cells[1, 4].Value = "Precio";
        worksheet.Cells[1, 5].Value = "Stock";
        worksheet.Cells[1, 6].Value = "Nombre";
        worksheet.Cells[1, 7].Value = "Apellido";
        worksheet.Cells[1, 8].Value = "Email";
        worksheet.Cells[1, 9].Value = "Telefono";
        worksheet.Cells[1, 10].Value = "Direccion";
        worksheet.Cells[1, 11].Value = "Documento";
        worksheet.Cells[1, 12].Value = "CantidadVendida";
        worksheet.Cells[1, 13].Value = "Fecha";

        // Estilo de encabezados
        using (var range = worksheet.Cells[1, 1, 1, 13])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
        }

        // Datos de ejemplo
        worksheet.Cells[2, 1].Value = "PROD001";
        worksheet.Cells[2, 2].Value = "Laptop HP";
        worksheet.Cells[2, 3].Value = "Laptop HP 15.6 pulgadas";
        worksheet.Cells[2, 4].Value = 850.00;
        worksheet.Cells[2, 5].Value = 10;
        worksheet.Cells[2, 6].Value = "Juan";
        worksheet.Cells[2, 7].Value = "Pérez";
        worksheet.Cells[2, 8].Value = "juan.perez@email.com";
        worksheet.Cells[2, 9].Value = "555-1234";
        worksheet.Cells[2, 10].Value = "Calle Principal 123";
        worksheet.Cells[2, 11].Value = "12345678";
        worksheet.Cells[2, 12].Value = 2;
        worksheet.Cells[2, 13].Value = DateTime.Now.ToString("yyyy-MM-dd");

        worksheet.Cells.AutoFitColumns();

        package.SaveAs(new FileInfo(filePath));
    }
}
