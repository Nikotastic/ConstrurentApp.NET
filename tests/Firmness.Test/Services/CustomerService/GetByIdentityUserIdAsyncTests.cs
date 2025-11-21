using Firmness.Application.Interfaces;
using Firmness.Application.Services;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Firmness.Test.Services.CustomerService;

public class GetByIdentityUserIdAsyncTests
{
    // Test Get By IdentityUser IdAsync method
    [Fact]
    public async Task GetByIdentityUserIdAsync_ValidUserId_ReturnsCustomer()
    {
        // Arrange
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockLogger = new Mock<ILogger<Application.Services.CustomerService>>();
        var service = new Application.Services.CustomerService(mockCustomerRepo.Object, mockNotificationService.Object, mockLogger.Object);

        var userId = "user123";
        var customer = new Customer("John", "Doe", "john@example.com") { Phone = "1234567890", Address = "Address 123" };

        mockCustomerRepo.Setup(repo => repo.GetByIdentityUserIdAsync(userId))
            .ReturnsAsync(customer);

        // Act
        var result = await service.GetByIdentityUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customer.Email, result.Email);
    }

    // Test Get By IdentityUser IdAsync method with invalid user id
    [Fact]
    public async Task GetByIdentityUserIdAsync_InvalidUserId_ReturnsNull()
    {
        // Arrange
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockLogger = new Mock<ILogger<Application.Services.CustomerService>>();
        var service = new Application.Services.CustomerService(mockCustomerRepo.Object, mockNotificationService.Object, mockLogger.Object);

        mockCustomerRepo.Setup(repo => repo.GetByIdentityUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await service.GetByIdentityUserIdAsync("invalid");

        // Assert
        Assert.Null(result);
    }
}
