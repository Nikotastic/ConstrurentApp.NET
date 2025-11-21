using Firmness.Application.Interfaces;
using Firmness.Domain.Common;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Firmness.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepo;
    private readonly INotificationService _notificationService;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        ICustomerRepository customerRepo,
        INotificationService notificationService,
        ILogger<CustomerService> logger)
    {
        _customerRepo = customerRepo;
        _notificationService = notificationService;
        _logger = logger;
    }
    
    // CRUD operations
    public Task<Customer?> GetByIdAsync(Guid id) => _customerRepo.GetByIdAsync(id);

    public Task<Customer?> GetByIdentityUserIdAsync(string identityUserId) 
        => _customerRepo.GetByIdentityUserIdAsync(identityUserId);

    public Task<IEnumerable<Customer>> GetAllAsync() => _customerRepo.GetAllAsync();
    
    public Task<IPaginatedResult<Customer>> GetAllAsync(int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);
        return _customerRepo.GetAllAsync(page, pageSize);
    }

    public async Task AddAsync(Customer customer)
    {
        if (customer is null) throw new ArgumentNullException(nameof(customer));
        if (string.IsNullOrWhiteSpace(customer.Email)) 
            throw new ArgumentException("Email required", nameof(customer.Email));
        
        // Add the client
        await _customerRepo.AddAsync(customer);
        
        // Send welcome email
        try
        {
            await _notificationService.SendWelcomeEmailAsync(customer);
            
            _logger.LogInformation(
                "Welcome email sent to {CustomerName} ({Email})",
                customer.FullName,
                customer.Email);
        }
        catch (Exception ex)
        {
            // Log but the operation doesn't fail if the email fails
            _logger.LogError(
                ex,
                "Error sending welcome email to {Email}",
                customer.Email);
        }
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