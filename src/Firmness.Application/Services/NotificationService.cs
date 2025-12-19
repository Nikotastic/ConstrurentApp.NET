using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Firmness.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IEmailService _emailService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IEmailService emailService,
        ILogger<NotificationService> logger)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

   // Send welcome email
    public async Task SendWelcomeEmailAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Preparing welcome email for customer {CustomerId} - {CustomerName}",
                customer.Id,
                customer.FullName);

            var emailMessage = EmailMessage.CreateWelcomeEmail(
                customer.Email,
                customer.FullName);

            await _emailService.SendEmailAsync(emailMessage, cancellationToken);

            _logger.LogInformation(
                "Welcome email sent successfully to {Email}",
                customer.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send welcome email to customer {CustomerId}",
                customer.Id);
            throw;
        }
    }
    
    // Send bulk welcome emails
    public async Task SendBulkWelcomeEmailsAsync(IEnumerable<Customer> customers, CancellationToken cancellationToken = default)
    {
        try
        {
            var customersList = customers.ToList();
            _logger.LogInformation(
                "Preparing bulk welcome emails for {Count} customers",
                customersList.Count);

            var emailMessages = customersList
                .Where(c => !string.IsNullOrWhiteSpace(c.Email))
                .Select(customer => EmailMessage.CreateWelcomeEmail(
                    customer.Email,
                    customer.FullName))
                .ToList();

            if (emailMessages.Any())
            {
                await _emailService.SendBulkEmailAsync(emailMessages, cancellationToken);

                _logger.LogInformation(
                    "Bulk welcome emails sent successfully to {Count} customers",
                    emailMessages.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send bulk welcome emails");
            throw;
        }
    }
    
    // Send purchase confirmation email
    public async Task SendPurchaseConfirmationAsync(
        Customer customer,
        decimal totalAmount,
        string invoiceNumber,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Preparing purchase confirmation email for invoice {InvoiceNumber}",
                invoiceNumber);

            var emailMessage = EmailMessage.CreatePurchaseConfirmation(
                customer.Email,
                customer.FullName,
                totalAmount,
                invoiceNumber);

            await _emailService.SendEmailAsync(emailMessage, cancellationToken);

            _logger.LogInformation(
                "Purchase confirmation sent to {Email} for invoice {InvoiceNumber}",
                customer.Email,
                invoiceNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send purchase confirmation for invoice {InvoiceNumber}",
                invoiceNumber);
            throw;
        }
    }
    // Send vehicle rental confirmation email
    public async Task SendVehicleRentalConfirmationAsync(
        Customer customer,
        string vehicleName,
        DateTime startDate,
        DateTime endDate,
        decimal totalAmount,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Preparing vehicle rental confirmation for {VehicleName}",
                vehicleName);

            var subject = $"Income Confirmation - {vehicleName}";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #3498db;'>¡Rental Confirmed!</h2>
                    <p>Dear <strong>{customer.FullName}</strong>,</p>
                    <p>Your vehicle reservation has been confirmed.</p>
                    <div style='background-color: #ecf0f1; padding: 15px; margin: 20px 0; border-radius: 5px;'>
                        <p><strong>Vehicle:</strong> {vehicleName}</p>
                        <p><strong>Start Date:</strong> {startDate:dd/MM/yyyy}</p>
                        <p><strong>End Date:</strong> {endDate:dd/MM/yyyy}</p>
                        <p><strong>Total:</strong> ${totalAmount:N2}</p>
                    </div>
                    <p>Please present this email when you pick up the vehicle.</p>
                    <br/>
                    <p style='color: #7f8c8d;'>Greetings,<br/>The Firmness Team</p>
                </body>
                </html>";

            var emailMessage = new EmailMessage(
                customer.Email,
                subject,
                body,
                isHtml: true);

            await _emailService.SendEmailAsync(emailMessage, cancellationToken);

            _logger.LogInformation(
                "Vehicle rental confirmation sent to {Email}",
                customer.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send vehicle rental confirmation to {Email}",
                customer.Email);
            throw;
        }
    }

    // Send vehicle return reminder email
    public async Task SendVehicleReturnReminderAsync(
        Customer customer,
        string vehicleName,
        DateTime returnDate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Preparing vehicle return reminder for {VehicleName}",
                vehicleName);

            var subject = $"Return Reminder - {vehicleName}";

            var body = $@"

            <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #e74c3c;'>Return Reminder</h2>
                    <p>Dear <strong>{customer.FullName}</strong>,</p>
                    <p>We remind you that you must return the vehicle <strong>{vehicleName}</strong>.</p>
                    <div style='background-color: #ffe5e5; padding: 15px; margin: 20px 0; border-radius: 5px;'>
                        <p><strong>Return Date:</strong> {returnDate:dd/MM/yyyy HH:mm}</p>
                    </div>
                    <p>To avoid additional charges, please return the vehicle on time.</p>
                    <br/>
                    <p style='color: #7f8c8d;'>Greetings,<br/>The Firmness Team</p>
                </body>
            </html>";

            var emailMessage = new EmailMessage(
                customer.Email,
                subject,
                body,
                isHtml: true);

            await _emailService.SendEmailAsync(emailMessage, cancellationToken);

            _logger.LogInformation(
                "Vehicle return reminder sent to {Email}",
                customer.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send vehicle return reminder to {Email}",
                customer.Email);
            throw;
        }
    }
    // Send bulk vehicle return reminders
    public async Task SendBulkVehicleReturnRemindersAsync(
        IEnumerable<(Customer customer, string vehicleName, DateTime returnDate)> rentals,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var rentalsList = rentals.ToList();
            _logger.LogInformation(
                "Preparing bulk vehicle return reminders for {Count} rentals",
                rentalsList.Count);

            var emailMessages = rentalsList
                .Where(r => r.customer != null && !string.IsNullOrWhiteSpace(r.customer.Email))
                .Select(rental =>
                {
                    var subject = $"Return Reminder - {rental.vehicleName}";
                    var body = $@"
                    <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <h2 style='color: #e74c3c;'>Return Reminder</h2>
                            <p>Dear <strong>{rental.customer.FullName}</strong>,</p>
                            <p>We remind you that you must return the vehicle <strong>{rental.vehicleName}</strong>.</p>
                            <div style='background-color: #ffe5e5; padding: 15px; margin: 20px 0; border-radius: 5px;'>
                                <p><strong>Return Date:</strong> {rental.returnDate:dd/MM/yyyy HH:mm}</p>
                            </div>
                            <p>To avoid additional charges, please return the vehicle on time.</p>
                            <br/>
                            <p style='color: #7f8c8d;'>Greetings,<br/>The Firmness Team</p>
                        </body>
                    </html>";

                    return new EmailMessage(
                        rental.customer.Email,
                        subject,
                        body,
                        isHtml: true);
                })
                .ToList();

            if (emailMessages.Any())
            {
                await _emailService.SendBulkEmailAsync(emailMessages, cancellationToken);

                _logger.LogInformation(
                    "Bulk vehicle return reminders sent successfully to {Count} customers",
                    emailMessages.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send bulk vehicle return reminders");
            throw;
        }
    }
    
    // Send account activation email
    public async Task SendAccountActivationEmailAsync(Customer customer, string activationLink, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Preparing account activation email for customer {CustomerId} - {CustomerName}",
                customer.Id,
                customer.FullName);

            var emailMessage = EmailMessage.CreateAccountActivationEmail(
                customer.Email,
                customer.FullName,
                activationLink);

            await _emailService.SendEmailAsync(emailMessage, cancellationToken);

            _logger.LogInformation(
                "Account activation email sent successfully to {Email}",
                customer.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send account activation email to customer {CustomerId}",
                customer.Id);
            throw;
        }
    }

    // Send password reset email
    public async Task SendPasswordResetEmailAsync(string email, string userName, string resetLink, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Preparing password reset email for {Email}",
                email);

            var emailMessage = EmailMessage.CreatePasswordResetEmail(
                email,
                userName,
                resetLink);

            await _emailService.SendEmailAsync(emailMessage, cancellationToken);

            _logger.LogInformation(
                "Password reset email sent successfully to {Email}",
                email);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send password reset email to {Email}",
                email);
            throw;
        }
    }
}
