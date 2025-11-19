using Firmness.Web.ViewModels.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;
using AutoMapper;

namespace Firmness.Web.Controllers
{
    [Authorize(Roles="Admin")] 
    public class CustomersController : Controller
    {
        private readonly ICustomerService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICustomerService service, IMapper mapper, ILogger<CustomersController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        // GET: /Customers
        public async Task<IActionResult> Index(string? q, int page = 1)
        {
            const int pageSize = 10; 
            page = Math.Max(1, page);

            try
            {
                if (!string.IsNullOrWhiteSpace(q))
                {
                    var allCustomers = await _service.GetAllAsync();
                    var term = q.Trim();
                    var filtered = allCustomers
                        .Where(c =>
                            (!string.IsNullOrWhiteSpace(c.FirstName) && c.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrWhiteSpace(c.LastName)  && c.LastName.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrWhiteSpace(c.Email)     && c.Email.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrWhiteSpace(c.Phone)     && c.Phone.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrWhiteSpace(c.Document)  && c.Document.Contains(term, StringComparison.OrdinalIgnoreCase))
                        )
                        .ToList();
                    
                    ViewData["Query"] = q;
                    
                    // Create paginated result
                    var pagedFiltered = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                    var paginatedResult = new Firmness.Domain.Common.PaginatedResult<Customer>(
                        pagedFiltered, 
                        page, 
                        pageSize, 
                        filtered.Count);
                    
                    return View(paginatedResult);
                }
                var paginatedCustomers = await _service.GetAllAsync(page, pageSize);
                ViewData["Query"] = q;
                return View(paginatedCustomers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customers");
                TempData["Error"] = "Can't load customers.";
                var empty = Firmness.Domain.Common.PaginatedResult<Customer>.Empty(page, pageSize);
                return View(empty);
            }
        }
        
        
        // GET: /Customers/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var customer = await _service.GetByIdAsync(id);
            if (customer == null) return NotFound();
            
            // Map entity to view model
            var viewModel = _mapper.Map<CustomerFormViewModel>(customer);
            return View(viewModel);
        }

        // GET: /Customers/Create
        public IActionResult Create() => View(new CustomerFormViewModel());

        // POST: /Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerFormViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                if (!IsValidDocument(vm.Document))
                {
                    ModelState.AddModelError(nameof(vm.Document), "The document does not have a valid format.");
                    return View(vm);
                }

                // Map view model to entity
                var entity = _mapper.Map<Customer>(vm);
                
                await _service.AddAsync(entity);

                TempData["Success"] = $"Client created successfully. Id: {entity.Id}";
                return RedirectToAction(nameof(Index));
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error creating customer.");
                ModelState.AddModelError(string.Empty, "A database error occurred while creating the client.");
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer.");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the client. Please try again.");
                return View(vm);
            }
        }


       // GET: /Customers/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var customer = await _service.GetByIdAsync(id);
            if (customer == null) return NotFound();
            var viewModel = _mapper.Map<CustomerFormViewModel>(customer);
            return View(viewModel);
        }

        // POST: /Customers/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CustomerFormViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            try
            {
                if (!IsValidDocument(vm.Document))
                {
                    ModelState.AddModelError(nameof(vm.Document), "The document does not have a valid format.");
                    return View(vm);
                }

                var customer = await _service.GetByIdAsync(id);
                if (customer == null) return NotFound();

                // Map view model to entity
                _mapper.Map(vm, customer);      

                await _service.UpdateAsync(customer);
                TempData["Success"] = "Client update correctly.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer {CustomerId}", id);
                ModelState.AddModelError(string.Empty, "An error occurred while updating the client.");
                return View(vm);
            }
        }

        // GET: /Customers/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var customer = await _service.GetByIdAsync(id);
            if (customer == null) return NotFound();
            var viewModel = _mapper.Map<CustomerFormViewModel>(customer);
            return View(viewModel);
        }

        // POST: /Customers/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                TempData["Success"] = "Client successfully removed.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer {CustomerId}", id);
                TempData["Error"] = "An error occurred while deleting the client.";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // GET: Customers/ExportToExcel
        public async Task<IActionResult> ExportToExcel([FromServices] IExportService exportService)
        {
            try
            {
                var excelBytes = await exportService.ExportCustomersToExcelAsync();
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    $"Clients_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx", enableRangeProcessing: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting customers to Excel");
                TempData["Error"] = "Could not export to Excel.";
                return RedirectToAction(nameof(Index));
            }
        }
        
        // GET: Customers/ExportToPdf
        public async Task<IActionResult> ExportToPdf([FromServices] IExportService exportService)
        {
            try
            {
                var pdfBytes = await exportService.ExportCustomersToPdfAsync();
                return File(pdfBytes, "application/pdf", $"Clients_{DateTime.Now:yyyyMMdd_HHmmss}.pdf", enableRangeProcessing: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting customers to PDF");
                TempData["Error"] = "Could not export to Excel.";
                return RedirectToAction(nameof(Index));
            }
        }


        private static bool IsValidDocument(string? doc)
        {
            if (string.IsNullOrWhiteSpace(doc)) return false;
            var trimmed = doc.Trim();
            return System.Text.RegularExpressions.Regex.IsMatch(trimmed, @"^[\d\-]+$");
        }
    }
}