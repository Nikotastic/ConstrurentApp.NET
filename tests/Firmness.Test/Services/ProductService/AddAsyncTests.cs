using Firmness.Application.Services;
using Firmness.Domain.Common;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Moq;

namespace Firmness.Test.Services.ProductService;

public class AddAsyncTests
{
    // Test AddAsync method
    [Fact]
    public async Task AddAsync_ValidProduct_ReturnsSuccess()
    {
        // Arrange
        var mockProductRepo = new Mock<IProductRepository>();
        var service = new Application.Services.ProductService(mockProductRepo.Object);

        var product = new Product("SKU001", "New Product", "Description", 50m, "image.jpg", 20);

        mockProductRepo.Setup(repo => repo.AddAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await service.AddAsync(product);

        // Assert
        Assert.True(result.IsSuccess);
    }

    // Test AddAsync method with null product
    [Fact]
    public async Task AddAsync_NullProduct_ReturnsFailure()
    {
        // Arrange
        var mockProductRepo = new Mock<IProductRepository>();
        var service = new Application.Services.ProductService(mockProductRepo.Object);

        // Act
        var result = await service.AddAsync(null);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.Validation, result.ErrorCode);
    }

    // Test AddAsync method with negative price
    [Fact]
    public async Task AddAsync_NegativePrice_ReturnsFailure()
    {
        // Arrange
        var mockProductRepo = new Mock<IProductRepository>();
        var service = new Application.Services.ProductService(mockProductRepo.Object);

        var product = new Product("SKU001", "Product", "Desc", -10m, "img.jpg", 5);

        // Act
        var result = await service.AddAsync(product);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.Validation, result.ErrorCode);
    }

    // Test AddAsync method with empty name
    [Fact]
    public async Task AddAsync_EmptyName_ReturnsFailure()
    {
        // Arrange
        var mockProductRepo = new Mock<IProductRepository>();
        var service = new Application.Services.ProductService(mockProductRepo.Object);

        var product = new Product("SKU001", "", "Desc", 10m, "img.jpg", 5);

        // Act
        var result = await service.AddAsync(product);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.Validation, result.ErrorCode);
    }
}
