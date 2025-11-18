using Firmness.Domain.Entities;

namespace Firmness.Application.Interfaces;

public interface INotificationService
{
    // Send a welcome email to a new customer
    Task SendWelcomeEmailAsync(Customer customer, CancellationToken cancellationToken = default);
    
    // Send welcome emails to multiple customers
    Task SendBulkWelcomeEmailsAsync(IEnumerable<Customer> customers, CancellationToken cancellationToken = default);

    // Send purchase/sale confirmation

    Task SendPurchaseConfirmationAsync(
        Customer customer, 
        decimal totalAmount, 
        string invoiceNumber,
        CancellationToken cancellationToken = default);
    

    // Send vehicle rental notification

    Task SendVehicleRentalConfirmationAsync(
        Customer customer,
        string vehicleName,
        DateTime startDate,
        DateTime endDate,
        decimal totalAmount,
        CancellationToken cancellationToken = default);
    

    // Send vehicle return reminder

    Task SendVehicleReturnReminderAsync(
        Customer customer,
        string vehicleName,
        DateTime returnDate,
        CancellationToken cancellationToken = default);
    
    // Send vehicle return reminders to multiple customers
    Task SendBulkVehicleReturnRemindersAsync(
        IEnumerable<(Customer customer, string vehicleName, DateTime returnDate)> rentals,
        CancellationToken cancellationToken = default);
}

