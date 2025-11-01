using Firmness.Application.Interfaces;
using Firmness.Core.Entities;
using Firmness.Core.Interfaces;
namespace Firmness.Application.Services;

// Service for sales
public class SaleService : ISaleService
{
     // Inject repositories
    private readonly ISaleRepository _saleRepo;
    private readonly IProductRepository _productRepo;
    private readonly ISaleItemRepository _saleItemRepo;
    private readonly ICustomerRepository _customerRepo;
    // private readonly ILogger<SaleService> _logger;

    public SaleService(
        ISaleRepository saleRepo,
        IProductRepository productRepo,
        ISaleItemRepository saleItemRepo,
        ICustomerRepository customerRepo
        /*, ApplicationDbContext db removed to keep hexagonal boundaries */)
    {
        _saleRepo = saleRepo;
        _productRepo = productRepo;
        _saleItemRepo = saleItemRepo;
        _customerRepo = customerRepo;
        // _logger = logger;
    }

    // Create sale with validation of stock and persistence
    public async Task<Sale> CreateSaleAsync(Guid customerId, IEnumerable<(Guid productId, int quantity)> lines)
    {
        if (lines == null || !lines.Any()) throw new ArgumentException("There are no lines for sale", nameof(lines));

        var sale = new Sale
        {
            CustomerId = customerId,
            CreatedAt = DateTime.UtcNow,
            Items = new List<SaleItem>()
        };

        foreach (var (productId, qty) in lines)
        {
            var product = await _productRepo.GetByIdAsync(productId)
                ?? throw new InvalidOperationException($"Product {productId} not found");

            if (product.Stock < qty) throw new InvalidOperationException($"Stock is not enough for {product.Name}");

            // create sale item 
            var item = new SaleItem(productId, qty, product.Price);
            sale.Items.Add(item);

            // decrement stock 
            product.Stock -= qty;
            await _productRepo.UpdateAsync(product);
        }

        sale.TotalAmount = sale.Items.Sum(i => i.UnitPrice * i.Quantity);

        await _saleRepo.AddAsync(sale);

        return sale;
    }

    public Task<Sale?> GetByIdAsync(Guid id) => _saleRepo.GetByIdAsync(id);

    public Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId) => _saleRepo.GetByCustomerIdAsync(customerId);
}