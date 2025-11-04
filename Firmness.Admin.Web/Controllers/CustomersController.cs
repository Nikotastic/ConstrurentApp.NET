
using Firmness.Admin.Web.ViewModels.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Firmness.Application.Interfaces;
using Firmness.Core.Entities;

namespace Firmness.Admin.Web.Controllers
{
    [Authorize(Roles="Admin")] 
    public class CustomersController : Controller
    {
        private readonly ICustomerService _service;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICustomerService service, ILogger<CustomersController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        // GET: /Customers
        public async Task<IActionResult> Index() => View(await _service.GetAllAsync());
        
        
        // GET: /Customers/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var customer = await _service.GetByIdAsync(id);
            if (customer == null) return NotFound();
            return View(MapToVm(customer));
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
                    ModelState.AddModelError(nameof(vm.Document), "The document does not have a valid format..");
                    return View(vm);
                }

                var entity = new Customer(vm.FirstName.Trim(), vm.LastName.Trim(), vm.Email.Trim())
                {
                    Document = vm.Document.Trim(),
                    Phone = vm.Phone?.Trim() ?? string.Empty,
                    Address = vm.Address?.Trim() ?? string.Empty,
                    IsActive = vm.IsActive
                };

                await _service.AddAsync(entity);
                TempData["Success"] = "Client created successfully.";
                return RedirectToAction(nameof(Index));
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
            return View(MapToVm(customer));
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

                customer.FirstName = vm.FirstName.Trim();
                customer.LastName = vm.LastName.Trim();
                customer.Email = vm.Email.Trim();
                customer.Document = vm.Document.Trim();
                customer.Phone = vm.Phone?.Trim() ?? string.Empty;
                customer.Address = vm.Address?.Trim() ?? string.Empty;
                customer.IsActive = vm.IsActive;
                customer.UpdatedAt = DateTime.UtcNow;

                await _service.UpdateAsync(customer);
                TempData["Success"] = "Cliente actualizado correctamente.";
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
            return View(MapToVm(customer));
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

        private static CustomerFormViewModel MapToVm(Customer c) => new CustomerFormViewModel
        {
            Id = c.Id,
            FirstName = c.FirstName ?? string.Empty,
            LastName = c.LastName ?? string.Empty,
            Email = c.Email ?? string.Empty,
            Document = c.Document ?? string.Empty,
            Phone = string.IsNullOrWhiteSpace(c.Phone) ? null : c.Phone,
            Address = string.IsNullOrWhiteSpace(c.Address) ? null : c.Address,
            IsActive = c.IsActive
        };

        private static bool IsValidDocument(string? doc)
        {
            if (string.IsNullOrWhiteSpace(doc)) return false;
            var trimmed = doc.Trim();
            return System.Text.RegularExpressions.Regex.IsMatch(trimmed, @"^[\d\-]+$");
        }
    }
}