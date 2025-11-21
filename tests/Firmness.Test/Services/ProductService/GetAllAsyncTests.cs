using Firmness.Application.Services;
using Firmness.Domain.Common;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Moq;

namespace Firmness.Test.Services.ProductService;

public class GetAllAsyncTests
{
    // Test GetAllAsync method
    [Fact]
    public async Task GetAllAsync_ReturnsSuccessWithPaginatedResult()
    {
        // Arrange
        var mockProductRepo = new Mock<IProductRepository>();
        var service = new Application.Services.ProductService(mockProductRepo.Object);

        var products = new List<Product>
        {
            new Product("SKU001", "Product 1", "Desc1", 10m, "img1.jpg", 5),
            new Product("SKU002", "Product 2", "Desc2", 20m, "img2.jpg", 10)
        };

        mockProductRepo.Setup(repo => repo.GetPagedAsync(1, 50, null))
            .ReturnsAsync((products, 2L));

        // Act
        var result = await service.GetAllAsync(1, 50);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.TotalItems);
    }

    // Test GetAllAsync method with repository throws exception
    [Fact]
    public async Task GetAllAsync_RepositoryThrowsException_ReturnsFailure()
    {
        // Arrange
        var mockProductRepo = new Mock<IProductRepository>();
        var service = new Application.Services.ProductService(mockProductRepo.Object);

        mockProductRepo.Setup(repo => repo.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.ServerError, result.ErrorCode);
    }
}
