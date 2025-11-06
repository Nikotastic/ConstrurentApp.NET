using Firmness.Application.Interfaces;
using Firmness.Core.Common;
using Firmness.Core.Entities;
using Firmness.Core.Interfaces;

namespace Firmness.Application.Services;

// Services for customers in Application ()
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepo;
    public CustomerService(ICustomerRepository customerRepo) => _customerRepo = customerRepo;
    
    // CRUD operations
    public Task<Customer?> GetByIdAsync(Guid id) => _customerRepo.GetByIdAsync(id);

    public Task<IEnumerable<Customer>> GetAllAsync() => _customerRepo.GetAllAsync();
    
    public Task<IPaginatedResult<Customer>> GetAllAsync(int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);
        return _customerRepo.GetAllAsync(page, pageSize);
    }

    public Task AddAsync(Customer customer)
    {
        if (customer is null) throw new ArgumentNullException(nameof(customer));
        if (string.IsNullOrWhiteSpace(customer.Email)) throw new ArgumentException("Email required", nameof(customer.Email));
        return _customerRepo.AddAsync(customer);
    }

    public Task UpdateAsync(Customer customer)
    {
        if (customer is null) throw new ArgumentNullException(nameof(customer));
        return _customerRepo.UpdateAsync(customer);
    }

    public Task DeleteAsync(Guid id) => _customerRepo.DeleteAsync(id);
    
    public async Task<Result<long>> CountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _customerRepo.CountAsync(cancellationToken);
            return Result<long>.Success(count);
        }
        catch (Exception)
        {
            return Result<long>.Failure("An error occurred while counting customers.");
        }
    }

  
}