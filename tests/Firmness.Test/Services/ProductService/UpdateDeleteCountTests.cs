using Firmness.Application.Services;
using Firmness.Domain.Common;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Moq;

namespace Firmness.Test.Services.ProductService;

public class UpdateDeleteCountTests
{
    // Test UpdateAsync method
    [Fact]
    public async Task UpdateAsync_ValidProduct_ReturnsSuccess()
    {
        // Arrange
        var mockProductRepo = new Mock<IProductRepository>();
        var service = new Application.Services.ProductService(mockProductRepo.Object);

        var product = new Product("SKU001", "Updated Product", "Description", 60m, "image.jpg", 15);

        mockProductRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await service.UpdateAsync(product);

        // Assert
        Assert.True(result.IsSuccess);
    }

    // Test DeleteAsync method
    [Fact]
    public async Task DeleteAsync_ValidId_ReturnsSuccess()
    {
        // Arrange
        var mockProductRepo = new Mock<IProductRepository>();
        var service = new Application.Services.ProductService(mockProductRepo.Object);

        var productId = Guid.NewGuid();

        mockProductRepo.Setup(repo => repo.DeleteAsync(productId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await service.DeleteAsync(productId);

        // Assert
        Assert.True(result.IsSuccess);
    }

    // Test CountAsync method
    [Fact]
    public async Task CountAsync_ReturnsCount()
    {
        // Arrange
        var mockProductRepo = new Mock<IProductRepository>();
        var service = new Application.Services.ProductService(mockProductRepo.Object);

        var expectedCount = 42L;
        mockProductRepo.Setup(repo => repo.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCount);

        // Act
        var result = await service.CountAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedCount, result.Value);
    }
}
