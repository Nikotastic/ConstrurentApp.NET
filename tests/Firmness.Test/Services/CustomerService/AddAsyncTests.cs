using Firmness.Application.Interfaces;
using Firmness.Application.Services;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Firmness.Test.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace Firmness.Test.Services.CustomerService;

// Test AddAsync method 
public class AddAsyncTests
{
    [Fact]
    public async Task AddAsync_ValidCustomer_AddsAndSendsEmail()
    {
        // Arrange
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockEmailService = new MockEmailService();
        var mockNotificationLogger = new Mock<ILogger<Application.Services.NotificationService>>();
        var mockNotificationService = new Application.Services.NotificationService(mockEmailService, mockNotificationLogger.Object);
        var mockLogger = new Mock<ILogger<Application.Services.CustomerService>>();
        var service = new Application.Services.CustomerService(mockCustomerRepo.Object, mockNotificationService, mockLogger.Object);

        var customer = new Customer("John", "Doe", "john@example.com") { Phone = "1234567890", Address = "Address 123" };

        mockCustomerRepo.Setup(repo => repo.AddAsync(It.IsAny<Customer>()))
            .Returns(Task.CompletedTask);

        // Act
        await service.AddAsync(customer);

        // Assert
        mockCustomerRepo.Verify(repo => repo.AddAsync(customer), Times.Once);
        Assert.Single(mockEmailService.SentEmails);
        Assert.Equal("john@example.com", mockEmailService.SentEmails[0].To);
    }

    // Test AddAsync method with null customer
    [Fact]
    public async Task AddAsync_NullCustomer_ThrowsArgumentNullException()
    {
        // Arrange
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockLogger = new Mock<ILogger<Application.Services.CustomerService>>();
        var service = new Application.Services.CustomerService(mockCustomerRepo.Object, mockNotificationService.Object, mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.AddAsync(null!));
    }

    // Test AddAsync method with empty email
    [Fact]
    public async Task AddAsync_EmptyEmail_ThrowsArgumentException()
    {
        // Arrange
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockLogger = new Mock<ILogger<Application.Services.CustomerService>>();
        var service = new Application.Services.CustomerService(mockCustomerRepo.Object, mockNotificationService.Object, mockLogger.Object);

        var customer = new Customer("John", "Doe", "") { Phone = "1234567890", Address = "Address 123" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.AddAsync(customer));
    }
}
