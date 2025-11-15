using AutoMapper;
using Firmness.Web.ViewModels.Category;
using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmness.Web.Controllers;

[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly ICategoryService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ICategoryService service, IMapper mapper, ILogger<CategoriesController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // GET: /Categories - With pagination and search
    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        const int pageSize = 10;
        page = Math.Max(1, page);

        try
        {
            if (!string.IsNullOrWhiteSpace(q))
            {
                var allCategories = await _service.GetAllAsync();
                var term = q.Trim();
                var filtered = allCategories
                    .Where(c =>
                        (!string.IsNullOrWhiteSpace(c.Name) && c.Name.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrWhiteSpace(c.Description) && c.Description.Contains(term, StringComparison.OrdinalIgnoreCase))
                    )
                    .ToList();

                ViewData["Query"] = q;

                // Create paginated result
                var pagedFiltered = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                var paginatedResult = new Firmness.Domain.Common.PaginatedResult<Category>(
                    pagedFiltered,
                    page,
                    pageSize,
                    filtered.Count);

                return View(paginatedResult);
            }

            // Get paginated categories
            var paginatedCategories = await _service.GetAllAsync(page, pageSize);
            ViewData["Query"] = q;
            return View(paginatedCategories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading categories");
            TempData["Error"] = "Can't load categories.";
            var empty = Firmness.Domain.Common.PaginatedResult<Category>.Empty(page, pageSize);
            return View(empty);
        }
    }

    // GET: /Categories/Details/{id}
    public async Task<IActionResult> Details(Guid id)
    {
        var category = await _service.GetByIdAsync(id);
        if (category == null) return NotFound();

        var viewModel = _mapper.Map<CategoryFormViewModel>(category);
        return View(viewModel);
    }

    // GET: /Categories/Create
    public IActionResult Create() => View(new CategoryFormViewModel());

    // POST: /Categories/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var entity = _mapper.Map<Category>(vm);
            var result = await _service.AddAsync(entity);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Create category failed: {Message}", result.ErrorMessage);
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "An error occurred while creating the category.");
                return View(vm);
            }

            TempData["Success"] = "Category created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the category.");
            return View(vm);
        }
    }

    // GET: /Categories/Edit/{id}
    public async Task<IActionResult> Edit(Guid id)
    {
        var category = await _service.GetByIdAsync(id);
        if (category == null) return NotFound();

        var viewModel = _mapper.Map<CategoryFormViewModel>(category);
        return View(viewModel);
    }

    // POST: /Categories/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CategoryFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null) return NotFound();

            category.Name = vm.Name;
            category.Description = vm.Description ?? string.Empty;
            category.IsActive = vm.IsActive;

            var result = await _service.UpdateAsync(category);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Update category failed: {Message}", result.ErrorMessage);
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "An error occurred while updating the category.");
                return View(vm);
            }

            TempData["Success"] = "Category updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}", id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the category.");
            return View(vm);
        }
    }

    // GET: /Categories/Delete/{id}
    public async Task<IActionResult> Delete(Guid id)
    {
        var category = await _service.GetByIdAsync(id);
        if (category == null) return NotFound();

        var viewModel = _mapper.Map<CategoryFormViewModel>(category);
        return View(viewModel);
    }

    // POST: /Categories/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            var result = await _service.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.ErrorMessage ?? "Category could not be removed.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            TempData["Success"] = "Category successfully removed.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {CategoryId}", id);
            TempData["Error"] = "An error occurred while deleting the category.";
            return RedirectToAction(nameof(Delete), new { id });
        }
    }

    // POST: /Categories/ToggleActive/{id}
    [HttpPost]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        try
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null) return NotFound();

            category.IsActive = !category.IsActive;
            var result = await _service.UpdateAsync(category);

            if (result.IsSuccess)
            {
                return Json(new { success = true, isActive = category.IsActive });
            }

            return Json(new { success = false, message = result.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling category {CategoryId}", id);
            return Json(new { success = false, message = "Error changing state" });
        }
    }
}
