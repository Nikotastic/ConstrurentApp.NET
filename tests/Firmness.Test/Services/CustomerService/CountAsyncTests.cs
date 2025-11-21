using Firmness.Application.Interfaces;
using Firmness.Application.Services;
using Firmness.Domain.Common;
using Firmness.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Firmness.Test.Services.CustomerService;

public class CountAsyncTests
{
    // Test CountAsync method
    [Fact]
    public async Task CountAsync_Success_ReturnsCount()
    {
        // Arrange
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockLogger = new Mock<ILogger<Application.Services.CustomerService>>();
        var service = new Application.Services.CustomerService(mockCustomerRepo.Object, mockNotificationService.Object, mockLogger.Object);

        var expectedCount = 10L;
        mockCustomerRepo.Setup(repo => repo.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCount);

        // Act
        var result = await service.CountAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedCount, result.Value);
    }

    // Test CountAsync method with repository throws exception
    [Fact]
    public async Task CountAsync_RepositoryThrowsException_ReturnsFailure()
    {
        // Arrange
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockLogger = new Mock<ILogger<Application.Services.CustomerService>>();
        var service = new Application.Services.CustomerService(mockCustomerRepo.Object, mockNotificationService.Object, mockLogger.Object);

        mockCustomerRepo.Setup(repo => repo.CountAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await service.CountAsync();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.ServerError, result.ErrorCode);
    }
}
