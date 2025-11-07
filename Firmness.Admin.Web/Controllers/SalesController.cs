using Firmness.Admin.Web.ViewModels.Sales;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Firmness.Core.Entities;
using Firmness.Infrastructure.Data;

namespace Firmness.Admin.Web.Controllers
{
    public class SalesController : Controller
    {
        
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SalesController> _logger;

        public SalesController(ApplicationDbContext context, ILogger<SalesController> logger)
        {
            _context = context;
            _logger = logger;
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
