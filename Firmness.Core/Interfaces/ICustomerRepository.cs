using Firmness.Core.Entities;

namespace Firmness.Core.Interfaces;

// interface for customer repository
public interface ICustomerRepository
{
    // Crud operations
    Task<Customer?> GetByIdAsync(Guid id);
    Task<Customer?> GetByIdentityUserIdAsync(string identityUserId);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<IEnumerable<Customer>> SearchAsync(string? query);
    // search by name or document. page and pageSize are optional
    Task<IEnumerable<Customer>> SearchByNameOrDocumentAsync(string? term, int page = 1, int pageSize = 50);
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(Guid customer);
    Task<bool> ExistsAsync(Guid id);
}

