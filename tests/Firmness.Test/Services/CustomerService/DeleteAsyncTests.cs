using Firmness.Application.Interfaces;
using Firmness.Application.Services;
using Firmness.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Firmness.Test.Services.CustomerService;

// Test DeleteAsync method
public class DeleteAsyncTests
{
    [Fact]
    public async Task DeleteAsync_ValidId_DeletesCustomer()
    {
        // Arrange
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockLogger = new Mock<ILogger<Application.Services.CustomerService>>();
        var service = new Application.Services.CustomerService(mockCustomerRepo.Object, mockNotificationService.Object, mockLogger.Object);

        var customerId = Guid.NewGuid();

        mockCustomerRepo.Setup(repo => repo.DeleteAsync(customerId))
            .Returns(Task.CompletedTask);

        // Act
        await service.DeleteAsync(customerId);

        // Assert
        mockCustomerRepo.Verify(repo => repo.DeleteAsync(customerId), Times.Once);
    }

    // Test DeleteAsync method with repository throws exception
    [Fact]
    public async Task DeleteAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockLogger = new Mock<ILogger<Application.Services.CustomerService>>();
        var service = new Application.Services.CustomerService(mockCustomerRepo.Object, mockNotificationService.Object, mockLogger.Object);

        var customerId = Guid.NewGuid();

        mockCustomerRepo.Setup(repo => repo.DeleteAsync(customerId))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.DeleteAsync(customerId));
    }
}
