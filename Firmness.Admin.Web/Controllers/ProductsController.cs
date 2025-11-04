using Firmness.Admin.Web.ViewModels.Product;
using Firmness.Application.Interfaces;
using Firmness.Core.Entities;
using Firmness.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Admin.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IProductService _service;
        private readonly ILogger<ProductsController> _logger;
        public ProductsController(IProductService service, ILogger<ProductsController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger;
        }

        // GET: Products
        // q = query; filter by SKU or Name or Description; simple pagination
        public async Task<IActionResult> Index(string? q, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 200);

            try
            {
                var result = string.IsNullOrWhiteSpace(q)
                    ? await _service.GetAllAsync(page, pageSize)
                    : await _service.SearchAsync(q.Trim(), page, pageSize);

                ViewData["Query"] = q;
                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing products.");
                TempData["Error"] = "NThe products could not be loaded. Please try again later.";
                // return empty paginated result to avoid null reference in view
                var empty = new Firmness.Core.Common.PaginatedResult<Product>(Enumerable.Empty<Product>(), 0, page, pageSize);
                return View(empty);
            }
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(MapToFormVm(product));
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View(new ProductFormViewModel());
        }


        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: /Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var entity = new Product(vm.SKU, vm.Name, vm.Description ?? string.Empty, vm.Price ?? 0m, vm.ImageUrl ?? string.Empty, vm.Stock ?? 0m);

                await _service.AddAsync(entity);
                TempData["Success"] = "Product created correctly.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product.");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the product. Please try again.");
                return View(vm);
            }
        }

        // GET: /Products/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(MapToFormVm(product));
        }

        // POST: /Products/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProductFormViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var product = await _service.GetByIdAsync(id);
                if (product == null) return NotFound();

                // apply changes
                product.SKU = vm.SKU;
                product.Name = vm.Name;
                product.Description = vm.Description ?? string.Empty;
                product.Price = vm.Price ?? product.Price;
                product.ImageUrl = vm.ImageUrl ?? product.ImageUrl;
                product.Stock = vm.Stock ?? product.Stock;
                product.UpdatedAt = DateTime.UtcNow;

                await _service.UpdateAsync(product);
                TempData["Success"] = "Product successfully updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", id);
                ModelState.AddModelError(string.Empty, "An error occurred while updating the product. Please try again.");
                return View(vm);
            }
        }

        // GET: /Products/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(MapToFormVm(product));
        }

        // POST: /Products/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                TempData["Success"] = "Product successfully removed.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                TempData["Error"] = "An error occurred while deleting the product..";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
        
        // Map entity -> viewmodel
        private static ProductFormViewModel MapToFormVm(Product p) => new ProductFormViewModel
        {
            Id = p.Id,
            SKU = p.SKU,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            Stock = p.Stock
        };
    }
}
