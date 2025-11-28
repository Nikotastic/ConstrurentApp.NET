using Firmness.Web.ViewModels.Product;
using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Firmness.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IExportService _exportService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<ProductsController> _logger;
        
        public ProductsController(
            IProductService productService, 
            ICategoryService categoryService,
            IExportService exportService,
            IFileStorageService fileStorageService,
            ILogger<ProductsController> logger)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));
            _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
            _logger = logger;
        }

        // GET: Products
        // q = query; categoryId = filter by category
        public async Task<IActionResult> Index(string? q, Guid? categoryId, int page = 1, int pageSize = 20)
        {
            _logger.LogInformation("=== INDEX PRODUCTS: page={Page}, pageSize={PageSize}, query={Query}, categoryId={CategoryId}", 
                page, pageSize, q, categoryId);
            
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 200);

            // Get categories for dropdown - always set this
            try
            {
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
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error loading categories for dropdown");
                ViewBag.Categories = new List<SelectListItem>();
            }

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
        public async Task<IActionResult> Create(ProductFormViewModel vm, IFormFile? imageFile)
        {
            _logger.LogInformation("=== CREATE PRODUCT: Started ===");
            _logger.LogInformation("ModelState Valid: {IsValid}", ModelState.IsValid);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid. Errors: {Errors}", 
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                var categories = await _categoryService.GetActiveAsync();
                vm.Categories = categories
                    .OrderBy(c => c.Name)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToList();
                return View(vm);
            }

            try
            {
                _logger.LogInformation("Creating product entity with SKU: {SKU}, Name: {Name}", vm.SKU, vm.Name);
                
                // Handle image upload
                string imageUrl = string.Empty;
                if (imageFile != null && imageFile.Length > 0)
                {
                    _logger.LogInformation("Uploading product image: {FileName}", imageFile.FileName);
                    try
                    {
                        imageUrl = await _fileStorageService.UploadFileAsync(
                            imageFile.FileName,
                            imageFile.OpenReadStream(),
                            imageFile.ContentType,
                            "products"
                        );
                        _logger.LogInformation("Image uploaded successfully: {ImageUrl}", imageUrl);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error uploading product image");
                        ModelState.AddModelError("imageFile", "Error al subir la imagen. Por favor intenta de nuevo.");
                        var categoriesError = await _categoryService.GetActiveAsync();
                        vm.Categories = categoriesError
                            .OrderBy(c => c.Name)
                            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                            .ToList();
                        return View(vm);
                    }
                }
                
                var entity = new Product(vm.SKU, vm.Name, vm.Description ?? string.Empty, 
                    vm.Price ?? 0m, imageUrl, vm.Stock ?? 0m)
                {
                    CategoryId = vm.CategoryId,
                    Cost = vm.Cost,
                    MinStock = vm.MinStock,
                    Barcode = vm.Barcode,
                    IsActive = vm.IsActive
                };

                _logger.LogInformation("Product entity created with Id: {ProductId} and ImageUrl: {ImageUrl}", entity.Id, entity.ImageUrl);
                _logger.LogInformation("Calling ProductService.AddAsync...");
                
                var result = await _productService.AddAsync(entity);
                
                _logger.LogInformation("ProductService.AddAsync returned. IsSuccess: {IsSuccess}, ErrorMessage: {ErrorMessage}", 
                    result.IsSuccess, result.ErrorMessage);
                
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

                _logger.LogInformation("Product created successfully! Setting TempData and redirecting to Index");
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
        public async Task<IActionResult> Edit(Guid id, ProductFormViewModel vm, IFormFile? imageFile)
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
                string oldImageUrl = product.ImageUrl;
                string? newImageUrl = null;

                // Handle image upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    _logger.LogInformation("Uploading new product image: {FileName}", imageFile.FileName);
                    try
                    {
                        newImageUrl = await _fileStorageService.UploadFileAsync(
                            imageFile.FileName,
                            imageFile.OpenReadStream(),
                            imageFile.ContentType,
                            "products"
                        );
                        
                        _logger.LogInformation("New image uploaded successfully: {ImageUrl}", newImageUrl);
                        
                        // Delete old image if it exists
                        if (!string.IsNullOrEmpty(oldImageUrl))
                        {
                            try
                            {
                                await _fileStorageService.DeleteFileAsync(oldImageUrl);
                                _logger.LogInformation("Old product image deleted: {OldImageUrl}", oldImageUrl);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Could not delete old product image: {OldImageUrl}", oldImageUrl);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error uploading product image");
                        ModelState.AddModelError("imageFile", "Error al subir la imagen. Por favor intenta de nuevo.");
                        var categoriesError = await _categoryService.GetActiveAsync();
                        vm.Categories = categoriesError
                            .OrderBy(c => c.Name)
                            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name, Selected = c.Id == vm.CategoryId })
                            .ToList();
                        return View(vm);
                    }
                }

                // apply changes
                product.SKU = vm.SKU;
                product.Name = vm.Name;
                product.Description = vm.Description ?? string.Empty;
                product.Price = vm.Price ?? product.Price;
                // Use the new uploaded image URL if available, otherwise keep the existing one from the form
                product.ImageUrl = newImageUrl ?? vm.ImageUrl ?? product.ImageUrl;
                product.Stock = vm.Stock ?? product.Stock;
                product.CategoryId = vm.CategoryId;
                product.Cost = vm.Cost;
                product.MinStock = vm.MinStock;
                product.Barcode = vm.Barcode;
                product.IsActive = vm.IsActive;
                
                _logger.LogInformation("Updating product {ProductId} with ImageUrl: {ImageUrl}", product.Id, product.ImageUrl);

                var updateResult = await _productService.UpdateAsync(product);
                if (!updateResult.IsSuccess)
                {
                    _logger.LogWarning("Update product failed: {Message}", updateResult.ErrorMessage);
                    ModelState.AddModelError(string.Empty, updateResult.ErrorMessage ?? "Error updating product.");
                    var categories = await _categoryService.GetActiveAsync();
                    vm.Categories = categories
                        .OrderBy(c => c.Name)
                        .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name, Selected = c.Id == vm.CategoryId })
                        .ToList();
                    return View(vm);
                }

                TempData["Success"] = "Product updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product.");
                ModelState.AddModelError(string.Empty, "Error updating product. Try again.");
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
                    TempData["Error"] = result.ErrorMessage ?? "Error deleting product.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Success"] = "Product deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product.");
                TempData["Error"] = "Error deleting product. Try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Products/ExportToExcel
        public async Task<IActionResult> ExportToExcel(Guid? categoryId = null)
        {
            try
            {
                var excelData = await _exportService.ExportProductsToExcelAsync(categoryId);
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Products_{DateTime.Now:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting products to Excel");
                TempData["Error"] = "Error exporting products to Excel.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Products/ExportToPdf
        public async Task<IActionResult> ExportToPdf(Guid? categoryId = null)
        {
            try
            {
                var pdfData = await _exportService.ExportProductsToPdfAsync(categoryId);
                return File(pdfData, "application/pdf", $"Products_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting products to PDF");
                TempData["Error"] = "Error exporting products to PDF.";
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
                CategoryName = p.Category?.Name,
                IsActive = p.IsActive
            };
        }
    }
}
