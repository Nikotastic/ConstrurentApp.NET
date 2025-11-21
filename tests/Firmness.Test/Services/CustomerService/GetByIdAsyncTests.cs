using Firmness.Application.Interfaces;
using Firmness.Application.Services;
using Firmness.Domain.Common;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Firmness.Test.Services.CustomerService;

public class GetByIdAsyncTests
{
    // Test GetByIdAsync method
    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsCustomer()
    {
        // Arrange
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockLogger = new Mock<ILogger<Application.Services.CustomerService>>();
        var service = new Application.Services.CustomerService(mockCustomerRepo.Object, mockNotificationService.Object, mockLogger.Object);

        var customer = new Customer("John", "Doe", "john@example.com") { Phone = "1234567890", Address = "Address 123" };
        var customerId = customer.Id;

        mockCustomerRepo.Setup(repo => repo.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        // Act
        var result = await service.GetByIdAsync(customerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customerId, result.Id);
        mockCustomerRepo.Verify(repo => repo.GetByIdAsync(customerId), Times.Once);
    }

    // Test GetByIdAsync method with invalid id
    [Fact]
    public async Task GetByIdAsync_InvalidId_ReturnsNull()
    {
        // Arrange
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockLogger = new Mock<ILogger<Application.Services.CustomerService>>();
        var service = new Application.Services.CustomerService(mockCustomerRepo.Object, mockNotificationService.Object, mockLogger.Object);

        var customerId = Guid.NewGuid();
        mockCustomerRepo.Setup(repo => repo.GetByIdAsync(customerId))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await service.GetByIdAsync(customerId);

        // Assert
        Assert.Null(result);
    }
}
