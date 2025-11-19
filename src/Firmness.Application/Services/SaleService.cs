﻿using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Firmness.Domain.Common;
namespace Firmness.Application.Services;

// Service for sales
public class SaleService : ISaleService
{
     // Inject repositories
    private readonly ISaleRepository _saleRepo;
    private readonly IProductRepository _productRepo;
    private readonly ISaleItemRepository _saleItemRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly IUnitOfWork _uow;
    // private readonly ILogger<SaleService> _logger;

    public SaleService(
        ISaleRepository saleRepo,
        IProductRepository productRepo,
        ISaleItemRepository saleItemRepo,
        ICustomerRepository customerRepo,
        IUnitOfWork uow
        /*, ApplicationDbContext db removed to keep hexagonal boundaries */)
    {
        _saleRepo = saleRepo ?? throw new ArgumentNullException(nameof(saleRepo));
        _productRepo = productRepo ?? throw new ArgumentNullException(nameof(productRepo));
        _saleItemRepo = saleItemRepo ?? throw new ArgumentNullException(nameof(saleItemRepo));
        _customerRepo = customerRepo ?? throw new ArgumentNullException(nameof(customerRepo));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        // _logger = logger;
    }

    // Create sale with validation of stock and persistence
    public async Task<Sale> CreateSaleAsync(Guid customerId, IEnumerable<(Guid productId, int quantity)> lines, CancellationToken cancellationToken = default)
    {
        if (lines == null || !lines.Any()) throw new ArgumentException("There are no lines for sale", nameof(lines));

        var sale = new Sale
        {
            CustomerId = customerId,
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

            // decrement stock (tracked by DbContext)
            product.Stock -= qty;
            await _productRepo.UpdateAsync(product);
        }

        sale.TotalAmount = sale.Items.Sum(i => i.UnitPrice * i.Quantity);

        await _uow.SaveChangesAsync(cancellationToken);

        return sale;
    }

    
    public Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _saleRepo.GetByIdAsync(id);

    public Task<Sale?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        => _saleRepo.GetByIdWithDetailsAsync(id);

    public Task<IEnumerable<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
        => _saleRepo.GetAllAsync();

    public Task<IEnumerable<Sale>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        => _saleRepo.GetAllWithDetailsAsync();

    public Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        => _saleRepo.GetByCustomerIdAsync(customerId);

    public async Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        if (sale == null) throw new ArgumentNullException(nameof(sale));
        await _saleRepo.UpdateAsync(sale);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _saleRepo.DeleteAsync(id);
    }

    public async Task<Result<long>> CountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _saleRepo.CountAsync(cancellationToken);
            return Result<long>.Success(count);
        }
        catch (Exception)
        {
            return Result<long>.Failure("An error occurred while counting sales.");
        }
    }
}