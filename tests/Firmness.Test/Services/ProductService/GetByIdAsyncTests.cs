using Firmness.Application.Services;
using Firmness.Domain.Common;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Moq;

namespace Firmness.Test.Services.ProductService;

public class GetByIdAsyncTests
{
    // Test GetByIdAsync method
    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsSuccessWithProduct()
    {
        // Arrange
        var mockProductRepo = new Mock<IProductRepository>();
        var service = new Application.Services.ProductService(mockProductRepo.Object);

        var product = new Product("SKU001", "Test Product", "Description", 99.99m, "image.jpg", 10);
        var productId = product.Id;

        mockProductRepo.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await service.GetByIdAsync(productId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Product", result.Value.Name);
    }

    // Test GetByIdAsync method with invalid id
    [Fact]
    public async Task GetByIdAsync_InvalidId_ReturnsFailure()
    {
        // Arrange
        var mockProductRepo = new Mock<IProductRepository>();
        var service = new Application.Services.ProductService(mockProductRepo.Object);

        var productId = Guid.NewGuid();
        mockProductRepo.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await service.GetByIdAsync(productId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.NotFound, result.ErrorCode);
    }
}
