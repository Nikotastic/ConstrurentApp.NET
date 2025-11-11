using Firmness.Admin.Web.ViewModels.Product;
using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;
using Firmness.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace Firmness.Admin.Web.Controllers
{
    public class ProductsController : Controller
    {
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
                var serviceResult = string.IsNullOrWhiteSpace(q)
                    ? await _service.GetAllAsync(page, pageSize)
                    : await _service.SearchAsync(q.Trim(), page, pageSize);

                if (!serviceResult.IsSuccess)
                {
                    _logger.LogWarning("Products listing failed: {Message}", serviceResult.ErrorMessage);
                    TempData["Error"] = "Los productos no pudieron cargarse. Intenta nuevamente más tarde.";
                    var empty = new Firmness.Domain.Common.PaginatedResult<Product>(Enumerable.Empty<Product>(), 0, page, pageSize);
                    return View(empty);
                }

                ViewData["Query"] = q;
                return View(serviceResult.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error listing products.");
                TempData["Error"] = "Los productos no pudieron cargarse. Intenta nuevamente más tarde.";
                var empty = new Firmness.Domain.Common.PaginatedResult<Product>(Enumerable.Empty<Product>(), 0, page, pageSize);
                return View(empty);
            }
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (!result.IsSuccess || result.Value == null) return NotFound();
            return View(MapToFormVm(result.Value));
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View(new ProductFormViewModel());
        }


        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var entity = new Product(vm.SKU, vm.Name, vm.Description ?? string.Empty, vm.Price ?? 0m, vm.ImageUrl ?? string.Empty, vm.Stock ?? 0m);

                var result = await _service.AddAsync(entity);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Create product failed: {Message}", result.ErrorMessage);
                    ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "No se pudo crear el producto.");
                    return View(vm);
                }

                TempData["Success"] = "Producto creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product.");
                ModelState.AddModelError(string.Empty, "Ocurrió un error al crear el producto. Intenta nuevamente.");
                return View(vm);
            }
        }

        // GET: /Products/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (!result.IsSuccess || result.Value == null) return NotFound();
            return View(MapToFormVm(result.Value));
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
                var getResult = await _service.GetByIdAsync(id);
                if (!getResult.IsSuccess || getResult.Value == null) return NotFound();

                var product = getResult.Value;

                // apply changes
                product.SKU = vm.SKU;
                product.Name = vm.Name;
                product.Description = vm.Description ?? string.Empty;
                product.Price = vm.Price ?? product.Price;
                product.ImageUrl = vm.ImageUrl ?? product.ImageUrl;
                product.Stock = vm.Stock ?? product.Stock;

                var updateResult = await _service.UpdateAsync(product);
                if (!updateResult.IsSuccess)
                {
                    _logger.LogWarning("Update product failed: {Message}", updateResult.ErrorMessage);
                    ModelState.AddModelError(string.Empty, updateResult.ErrorMessage ?? "No se pudo actualizar el producto.");
                    return View(vm);
                }

                TempData["Success"] = "Producto actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", id);
                ModelState.AddModelError(string.Empty, "Ocurrió un error al actualizar el producto. Intenta nuevamente.");
                return View(vm);
            }
        }

        // GET: /Products/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (!result.IsSuccess || result.Value == null) return NotFound();
            return View(MapToFormVm(result.Value));
        }

        // POST: /Products/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Delete product failed: {Message}", result.ErrorMessage);
                    TempData["Error"] = result.ErrorMessage ?? "Ocurrió un error al eliminar el producto.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                TempData["Success"] = "Producto eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                TempData["Error"] = "Ocurrió un error al eliminar el producto.";
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
