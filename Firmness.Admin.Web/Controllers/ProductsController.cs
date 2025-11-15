using Firmness.Web.ViewModels.Product;
using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Firmness.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<ProductsController> _logger;
        
        public ProductsController(
            IProductService productService, 
            ICategoryService categoryService,
            ILogger<ProductsController> logger)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _logger = logger;
        }

        // GET: Products
        // q = query; categoryId = filter by category
        public async Task<IActionResult> Index(string? q, Guid? categoryId, int page = 1, int pageSize = 20)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 200);

            try
            {
                // Use service method instead of direct DbContext access
                var result = await _productService.GetPagedWithCategoryAsync(page, pageSize, q, categoryId);
                
                if (!result.IsSuccess || result.Value == null)
                {
                    _logger.LogError("Error loading products: {Message}", result.ErrorMessage);
                    TempData["Error"] = "Los productos no pudieron cargarse. Intenta nuevamente más tarde.";
                    var empty = new Firmness.Domain.Common.PaginatedResult<Product>(Enumerable.Empty<Product>(), page, pageSize, 0);
                    return View(empty);
                }

                // Get categories for dropdown
                var categories = await _categoryService.GetActiveAsync();
                ViewBag.Categories = categories
                    .OrderBy(c => c.Name)
                    .Select(c => new SelectListItem 
                    { 
                        Value = c.Id.ToString(), 
                        Text = c.Name,
                        Selected = categoryId.HasValue && c.Id == categoryId.Value
                    })
                    .ToList();

                ViewData["Query"] = q;
                ViewData["CategoryId"] = categoryId;
                
                return View(result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error listing products.");
                TempData["Error"] = "Los productos no pudieron cargarse. Intenta nuevamente más tarde.";
                var empty = new Firmness.Domain.Common.PaginatedResult<Product>(Enumerable.Empty<Product>(), page, pageSize, 0);
                return View(empty);
            }
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Value == null) return NotFound();
            return View(MapToFormVm(result.Value));
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetActiveAsync();
            var vm = new ProductFormViewModel
            {
                Categories = categories
                    .OrderBy(c => c.Name)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToList()
            };
            return View(vm);
        }


        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetActiveAsync();
                vm.Categories = categories
                    .OrderBy(c => c.Name)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToList();
                return View(vm);
            }

            try
            {
                var entity = new Product(vm.SKU, vm.Name, vm.Description ?? string.Empty, 
                    vm.Price ?? 0m, vm.ImageUrl ?? string.Empty, vm.Stock ?? 0m)
                {
                    CategoryId = vm.CategoryId,
                    Cost = vm.Cost,
                    MinStock = vm.MinStock,
                    Barcode = vm.Barcode,
                    IsActive = vm.IsActive
                };

                var result = await _productService.AddAsync(entity);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Create product failed: {Message}", result.ErrorMessage);
                    ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "No se pudo crear el producto.");
                    var categories = await _categoryService.GetActiveAsync();
                    vm.Categories = categories
                        .OrderBy(c => c.Name)
                        .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                        .ToList();
                    return View(vm);
                }

                TempData["Success"] = "Producto creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product.");
                ModelState.AddModelError(string.Empty, "Ocurrió un error al crear el producto. Intenta nuevamente.");
                var categories = await _categoryService.GetActiveAsync();
                vm.Categories = categories
                    .OrderBy(c => c.Name)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToList();
                return View(vm);
            }
        }

        // GET: /Products/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Value == null) return NotFound();
            
            var vm = MapToFormVm(result.Value);
            var categories = await _categoryService.GetActiveAsync();
            vm.Categories = categories
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == vm.CategoryId
                })
                .ToList();
            
            return View(vm);
        }

        // POST: /Products/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProductFormViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetActiveAsync();
                vm.Categories = categories
                    .OrderBy(c => c.Name)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name, Selected = c.Id == vm.CategoryId })
                    .ToList();
                return View(vm);
            }

            try
            {
                var getResult = await _productService.GetByIdAsync(id);
                if (!getResult.IsSuccess || getResult.Value == null) return NotFound();

                var product = getResult.Value;

                // apply changes
                product.SKU = vm.SKU;
                product.Name = vm.Name;
                product.Description = vm.Description ?? string.Empty;
                product.Price = vm.Price ?? product.Price;
                product.ImageUrl = vm.ImageUrl ?? product.ImageUrl;
                product.Stock = vm.Stock ?? product.Stock;
                product.CategoryId = vm.CategoryId;
                product.Cost = vm.Cost;
                product.MinStock = vm.MinStock;
                product.Barcode = vm.Barcode;
                product.IsActive = vm.IsActive;

                var updateResult = await _productService.UpdateAsync(product);
                if (!updateResult.IsSuccess)
                {
                    _logger.LogWarning("Update product failed: {Message}", updateResult.ErrorMessage);
                    ModelState.AddModelError(string.Empty, updateResult.ErrorMessage ?? "No se pudo actualizar el producto.");
                    var categories = await _categoryService.GetActiveAsync();
                    vm.Categories = categories
                        .OrderBy(c => c.Name)
                        .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name, Selected = c.Id == vm.CategoryId })
                        .ToList();
                    return View(vm);
                }

                TempData["Success"] = "Producto actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product.");
                ModelState.AddModelError(string.Empty, "Ocurrió un error al actualizar el producto. Intenta nuevamente.");
                var categories = await _categoryService.GetActiveAsync();
                vm.Categories = categories
                    .OrderBy(c => c.Name)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name, Selected = c.Id == vm.CategoryId })
                    .ToList();
                return View(vm);
            }
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Value == null) return NotFound();
            return View(MapToFormVm(result.Value));
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var result = await _productService.DeleteAsync(id);
                if (!result.IsSuccess)
                {
                    TempData["Error"] = result.ErrorMessage ?? "No se pudo eliminar el producto.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Success"] = "Producto eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product.");
                TempData["Error"] = "Ocurrió un error al eliminar el producto. Intenta nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        private static ProductFormViewModel MapToFormVm(Product p)
        {
            return new ProductFormViewModel
            {
                Id = p.Id,
                SKU = p.SKU,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Cost = p.Cost,
                Stock = p.Stock,
                MinStock = p.MinStock,
                ImageUrl = p.ImageUrl,
                Barcode = p.Barcode,
                CategoryId = p.CategoryId,
                IsActive = p.IsActive
            };
        }
    }
}