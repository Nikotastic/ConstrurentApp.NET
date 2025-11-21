using Firmness.Application.Interfaces;
using Firmness.Application.Services;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Firmness.Test.Services.CustomerService;

public class GetAllAsyncTests
{
    // Test GetAllAsync method
    [Fact]
    public async Task GetAllAsync_ReturnsAllCustomers()
    {
        // Arrange
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockLogger = new Mock<ILogger<Application.Services.CustomerService>>();
        var service = new Application.Services.CustomerService(mockCustomerRepo.Object, mockNotificationService.Object, mockLogger.Object);

        var customers = new List<Customer>
        {
            new Customer("John", "Doe", "john@example.com") { Phone = "1234567890", Address = "Address 1" },
            new Customer("Jane", "Smith", "jane@example.com") { Phone = "0987654321", Address = "Address 2" }
        };

        mockCustomerRepo.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(customers);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    // Test GetAllAsync method with no customers
    [Fact]
    public async Task GetAllAsync_NoCustomers_ReturnsEmptyList()
    {
        // Arrange
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockLogger = new Mock<ILogger<Application.Services.CustomerService>>();
        var service = new Application.Services.CustomerService(mockCustomerRepo.Object, mockNotificationService.Object, mockLogger.Object);

        mockCustomerRepo.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(new List<Customer>());

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
