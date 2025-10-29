using Firmness.Core.Entities;
using Firmness.Core.Interfaces;

// Servicio de aplicación (caso de uso) en Application: ProductService
namespace Firmness.Application.Services;

// service for products
public class ProductService
{
    private readonly IProductRepository _productRepository;
    
    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    // CRUD operations
   public Task<Product?> GetByIdAsync(Guid id) => _productRepository.GetByIdAsync(id);

   public Task<IEnumerable<Product>> GetAllAsync() => _productRepository.GetAllAsync();

   public Task<IEnumerable<Product>> SearchAsync(string? query) => _productRepository.SearchAsync(query);

   public async Task AddAsync(Product product)
   {
       // validation
       if (product.Price < 0) throw new ArgumentException("UnitPrice must be >= 0");
       await _productRepository.AddAsync(product);
   }
   public Task UpdateAsync(Product product) => _productRepository.UpdateAsync(product);
   public Task DeleteAsync(Guid id) => _productRepository.DeleteAsync(id);
}