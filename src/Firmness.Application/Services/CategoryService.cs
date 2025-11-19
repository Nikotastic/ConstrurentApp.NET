using Firmness.Application.Interfaces;
using Firmness.Domain.Common;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;

namespace Firmness.Application.Services;


// Service for Category entity

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public Task<Category?> GetByIdAsync(Guid id)
    {
        return _repository.GetByIdAsync(id);
    }

    public Task<IEnumerable<Category>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public Task<IPaginatedResult<Category>> GetAllAsync(int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);
        return _repository.GetAllAsync(page, pageSize);
    }

    public Task<IEnumerable<Category>> GetActiveAsync()
    {
        return _repository.GetActiveAsync();
    }

    public async Task<Result<Category>> AddAsync(Category category)
    {
        try
        {
            if (category == null)
                return Result<Category>.Failure("The category can't be null.", "CATEGORY_NULL");

            if (string.IsNullOrWhiteSpace(category.Name))
                return Result<Category>.Failure("The category name is required.", "CATEGORY_NAME_REQUIRED");

            await _repository.AddAsync(category);
            return Result<Category>.Success(category);
        }
        catch (Exception ex)
        {
            return Result<Category>.Failure($"Error adding category: {ex.Message}", "CATEGORY_ADD_ERROR");
        }
    }

    public async Task<Result> UpdateAsync(Category category)
    {
        try
        {
            if (category == null)
                return Result.Failure("The category can't be null.", "CATEGORY_NULL");

            if (string.IsNullOrWhiteSpace(category.Name))
                return Result.Failure("The name is required", "CATEGORY_NAME_REQUIRED");

            var exists = await _repository.ExistsAsync(category.Id);
            if (!exists)
                return Result.Failure("The category doesn't exist", "CATEGORY_NOT_FOUND");

            await _repository.UpdateAsync(category);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error updating category: {ex.Message}", "CATEGORY_UPDATE_ERROR");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            var exists = await _repository.ExistsAsync(id);
            if (!exists)
                return Result.Failure("The category doesn't exist", "CATEGORY_NOT_FOUND");

            await _repository.DeleteAsync(id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting category: {ex.Message}", "CATEGORY_DELETE_ERROR");
        }
    }

    public async Task<Result<long>> CountAsync()
    {
        try
        {
            var count = await _repository.CountAsync();
            return Result<long>.Success(count);
        }
        catch (Exception ex)
        {
            return Result<long>.Failure($"Error counting categories: {ex.Message}", "CATEGORY_COUNT_ERROR");
        }
    }
}

