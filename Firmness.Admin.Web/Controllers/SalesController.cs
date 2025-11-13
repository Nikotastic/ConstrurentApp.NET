using Firmness.Admin.Web.ViewModels.Sales;
using Firmness.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Firmness.Domain.Entities;
using Firmness.Infrastructure.Data;

namespace Firmness.Admin.Web.Controllers
{
    public class SalesController : Controller
    {
        
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SalesController> _logger;
        private readonly IReceiptService _receiptService;

        public SalesController(ApplicationDbContext context, ILogger<SalesController> logger, IReceiptService receiptService)
        {
            _context = context;
            _logger = logger;
            _receiptService = receiptService;
        }

        private async Task<List<SelectListItem>> BuildCustomerList(object? selected = null)
        {
            var customers = await _context.Customers
                .AsNoTracking()
                .Select(c => new { c.Id, c.Email, c.FirstName, c.LastName })
                .ToListAsync();

            var selStr = selected?.ToString();

            var list = customers
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = string.IsNullOrWhiteSpace(c.Email) ? c.FirstName : c.Email,
                    Selected = selStr != null && selStr == c.Id.ToString()
                })
                .ToList();

            return list;
        }
        // GET: Sales
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Sales.Include(s => s.Customer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Sales/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var sale = await _context.Sales
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sale == null) return NotFound();

            var vm = MapToFormVm(sale);
            vm.Customers = await BuildCustomerList(vm.CustomerId);
            return View(vm);
        }

        // GET: Sales/Create
        public IActionResult Create()
        {
            var vm = new SaleFormViewModel
            {
                Customers = _context.Customers
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Email })
                    .ToList(),
                CreatedAt = DateTime.Today,
                TotalAmount = 0m 
            };
            return View(vm);
        }

        // POST: Sales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaleFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Customers = _context.Customers
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Email })
                    .ToList();
                return View(vm);
            }

            try
            {
                var sale = MapToEntity(vm);
                _context.Add(sale);
                await _context.SaveChangesAsync();
                
                // Generar recibo PDF automáticamente
                try
                {
                    await _receiptService.GenerateReceiptAsync(sale.Id);
                    _logger.LogInformation("Recibo generado para la venta {SaleId}", sale.Id);
                }
                catch (Exception exReceipt)
                {
                    _logger.LogWarning(exReceipt, "No se pudo generar el recibo para la venta {SaleId}", sale.Id);
                    // No fallar la venta si el recibo falla
                }

                TempData["Success"] = "Sale created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sale.");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the sale. Please try again.");
                vm.Customers = _context.Customers
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Email })
                    .ToList();
                return View(vm);
            }
        }

        // GET: Sales/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var sale = await _context.Sales.FindAsync(id);
            if (sale == null) return NotFound();

            var vm = MapToFormVm(sale);
            vm.Customers = _context.Customers
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Email })
                .ToList();
            return View(vm);
        }

        // POST: Sales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SaleFormViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                vm.Customers = _context.Customers
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Email })
                    .ToList();
                return View(vm);
            }

            try
            {
                var sale = await _context.Sales.FindAsync(id);
                if (sale == null) return NotFound();

                // apply changes
                sale.CustomerId = vm.CustomerId;
                sale.CreatedAt = vm.CreatedAt;
                sale.TotalAmount = vm.TotalAmount;

                _context.Update(sale);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Sale updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleExists(vm.Id)) return NotFound();
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating sale {SaleId}", id);
                ModelState.AddModelError(string.Empty, "An error occurred while updating the sale. Please try again.");
                vm.Customers = _context.Customers
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Email })
                    .ToList();
                return View(vm);
            }
        }

        // GET: Sales/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var sale = await _context.Sales.Include(s => s.Customer).FirstOrDefaultAsync(m => m.Id == id);
            if (sale == null) return NotFound();

            var vm = MapToFormVm(sale);
            vm.Customers = await BuildCustomerList(vm.CustomerId);
            return View(vm);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale != null)
            {
                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Sale deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Sale not found.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SaleExists(Guid id) => _context.Sales.Any(e => e.Id == id);
        
        // GET: Sales/DownloadReceipt/5
        public async Task<IActionResult> DownloadReceipt(Guid? id)
        {
            if (id == null) return NotFound();

            try
            {
                var pdfBytes = await _receiptService.GetReceiptBytesAsync(id.Value);
                return File(pdfBytes, "application/pdf", $"recibo_{id.Value.ToString().Substring(0, 8)}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading receipt for sale {SaleId}", id);
                TempData["Error"] = "No se pudo descargar el recibo.";
                return RedirectToAction(nameof(Index));
            }
        }
        
        // GET: Sales/ExportToExcel
        public async Task<IActionResult> ExportToExcel([FromServices] IExportService exportService)
        {
            try
            {
                var excelBytes = await exportService.ExportSalesToExcelAsync();
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    $"ventas_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx", enableRangeProcessing: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting sales to Excel");
                TempData["Error"] = "No se pudo exportar a Excel.";
                return RedirectToAction(nameof(Index));
            }
        }
        
        // GET: Sales/ExportToPdf
        public async Task<IActionResult> ExportToPdf([FromServices] IExportService exportService)
        {
            try
            {
                var pdfBytes = await exportService.ExportSalesToPdfAsync();
                return File(pdfBytes, "application/pdf", $"ventas_{DateTime.Now:yyyyMMdd_HHmmss}.pdf", enableRangeProcessing: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting sales to PDF");
                TempData["Error"] = "No se pudo exportar a PDF.";
                return RedirectToAction(nameof(Index));
            }
        }

        // helpers
        private static SaleFormViewModel MapToFormVm(Sale s) => new SaleFormViewModel
        {
            Id = s.Id,
            CustomerId = s.CustomerId,
            CreatedAt = s.CreatedAt,
            TotalAmount = s.TotalAmount,
            CustomerName = s.Customer?.FullName,
            CustomerEmail = s.Customer?.Email
        };

        private static Sale MapToEntity(SaleFormViewModel vm) => new Sale
        {
            CustomerId = vm.CustomerId,
            CreatedAt = vm.CreatedAt,
            TotalAmount = vm.TotalAmount
        };
    }
}
