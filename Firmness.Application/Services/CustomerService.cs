using Firmness.Application.Interfaces;
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

    public Task AddAsync(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.Email)) throw new ArgumentException("Email required", nameof(customer.Email));
        return _customerRepo.AddAsync(customer);
    }

    public Task UpdateAsync(Customer customer) => _customerRepo.UpdateAsync(customer);

    public Task DeleteAsync(Guid id) => _customerRepo.DeleteAsync(id);
    
    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _customerRepo.CountAsync(cancellationToken);
    }
}