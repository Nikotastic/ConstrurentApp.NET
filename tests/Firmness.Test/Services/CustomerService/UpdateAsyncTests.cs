using Firmness.Application.Interfaces;
using Firmness.Application.Services;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Firmness.Test.Services.CustomerService;

public class UpdateAsyncTests
{
    // Test UpdateAsync method
    [Fact]
    public async Task UpdateAsync_ValidCustomer_UpdatesCustomer()
    {
        // Arrange
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockLogger = new Mock<ILogger<Application.Services.CustomerService>>();
        var service = new Application.Services.CustomerService(mockCustomerRepo.Object, mockNotificationService.Object, mockLogger.Object);

        var customer = new Customer("John", "Doe", "john@example.com") { Phone = "1234567890", Address = "Address 123" };

        mockCustomerRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Customer>()))
            .Returns(Task.CompletedTask);

        // Act
        await service.UpdateAsync(customer);

        // Assert
        mockCustomerRepo.Verify(repo => repo.UpdateAsync(customer), Times.Once);
    }

    // Test UpdateAsync method with null customer
    [Fact]
    public async Task UpdateAsync_NullCustomer_ThrowsArgumentNullException()
    {
        // Arrange
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockLogger = new Mock<ILogger<Application.Services.CustomerService>>();
        var service = new Application.Services.CustomerService(mockCustomerRepo.Object, mockNotificationService.Object, mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateAsync(null!));
    }
}
