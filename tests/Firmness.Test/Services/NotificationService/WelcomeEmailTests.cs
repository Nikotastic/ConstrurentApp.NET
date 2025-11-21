using Firmness.Application.Interfaces;
using Firmness.Application.Services;
using Firmness.Domain.Entities;
using Firmness.Test.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace Firmness.Test.Services.NotificationService;

public class WelcomeEmailTests
{
    // Test Send Welcome Email Async method 
    [Fact]
    public async Task SendWelcomeEmailAsync_ValidCustomer_SendsEmail()
    {
        // Arrange
        var mockEmailService = new MockEmailService();
        var mockLogger = new Mock<ILogger<Application.Services.NotificationService>>();
        var service = new Application.Services.NotificationService(mockEmailService, mockLogger.Object);

        var customer = new Customer("John", "Doe", "john@example.com")
        {
            Phone = "1234567890",
            Address = "Address 123"
        };

        // Act
        await service.SendWelcomeEmailAsync(customer);

        // Assert
        Assert.Single(mockEmailService.SentEmails);
        Assert.Equal("john@example.com", mockEmailService.SentEmails[0].To);
        Assert.Contains("Welcome", mockEmailService.SentEmails[0].Subject);
    }

    // Test Send Bulk Welcome Email Async method
    [Fact]
    public async Task SendBulkWelcomeEmailsAsync_MultipleCustomers_SendsAllEmails()
    {
        // Arrange
        var mockEmailService = new MockEmailService();
        var mockLogger = new Mock<ILogger<Application.Services.NotificationService>>();
        var service = new Application.Services.NotificationService(mockEmailService, mockLogger.Object);

        var customers = new List<Customer>
        {
            new Customer("John", "Doe", "john@example.com") { Phone = "1234567890", Address = "Address 1" },
            new Customer("Jane", "Smith", "jane@example.com") { Phone = "0987654321", Address = "Address 2" }
        };

        // Act
        await service.SendBulkWelcomeEmailsAsync(customers);

        // Assert
        Assert.Equal(2, mockEmailService.SentEmails.Count);
    }
}
