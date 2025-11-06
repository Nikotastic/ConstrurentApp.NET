using Firmness.Application.Interfaces;
using Firmness.Core.Common;
using Firmness.Core.Entities;
using Firmness.Core.Interfaces;

namespace Firmness.Application.Services;

// service for products
public class ProductService : IProductService 
{
    // Inject repository
    private readonly IProductRepository _productRepository;
    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    // Crud operations
    public async Task<Result<Product>> GetByIdAsync(Guid id)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return Result<Product>.Failure("Product not found.", ErrorCodes.NotFound);
            return Result<Product>.Success(product);
        }
        catch (Exception)
        {
            return Result<Product>.Failure("An error occurred while retrieving the product.", ErrorCodes.ServerError);
        }
    }

    public async Task<Result<PaginatedResult<Product>>> GetAllAsync(int page = 1, int pageSize = 50)
    {
        try
        {
            var (items, total) = await _productRepository.GetPagedAsync(page, pageSize);
            var paginated = new PaginatedResult<Product>(items, page, pageSize, total);
            return Result<PaginatedResult<Product>>.Success(paginated);
        }
        catch (Exception)
        {
            return Result<PaginatedResult<Product>>.Failure("An error occurred while listing products.", ErrorCodes.ServerError);
        }
    }

    // Search products using query and return paginated result
    public async Task<Result<PaginatedResult<Product>>> SearchAsync(string? query, int page = 1, int pageSize = 50)
    {
        try
        {
            var (items, total) = await _productRepository.GetPagedAsync(page, pageSize, query);
            var paginated = new PaginatedResult<Product>(items, page, pageSize, total);
            return Result<PaginatedResult<Product>>.Success(paginated);
        }
        catch (Exception)
        {
            return Result<PaginatedResult<Product>>.Failure("An error occurred while searching products.", ErrorCodes.ServerError);
        }
    }

    public async Task<Result> AddAsync(Product? product)
    {
        // Basic validations
        if (product is null) return Result.Failure("Product is required.", ErrorCodes.Validation);
        if (product.Price < 0) return Result.Failure("Price must be non-negative.", ErrorCodes.Validation);
        if (string.IsNullOrWhiteSpace(product.Name)) return Result.Failure("Name is required.", ErrorCodes.Validation);

        try
        {
            await _productRepository.AddAsync(product);
            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure("An error occurred while creating the product.", ErrorCodes.ServerError);
        }
    }

    public async Task<Result> UpdateAsync(Product? product)
    {
        if (product is null) return Result.Failure("Product is required.", ErrorCodes.Validation);
        if (product.Price < 0) return Result.Failure("Price must be non-negative.", ErrorCodes.Validation);
        if (string.IsNullOrWhiteSpace(product.Name)) return Result.Failure("Name is required.", ErrorCodes.Validation);

        try
        {
            await _productRepository.UpdateAsync(product);
            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure("An error occurred while updating the product.", ErrorCodes.ServerError);
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            await _productRepository.DeleteAsync(id);
            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure("An error occurred while deleting the product.", ErrorCodes.ServerError);
        }
    }
    public async Task<Result<long>> CountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _productRepository.CountAsync(cancellationToken);
            return Result<long>.Success(count);
        }
        catch (Exception)
        {
            return Result<long>.Failure("An error occurred while counting products.", ErrorCodes.ServerError);
        }
    }
}