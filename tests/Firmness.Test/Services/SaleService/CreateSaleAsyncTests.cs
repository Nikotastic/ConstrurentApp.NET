using Firmness.Application.Services;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Moq;

namespace Firmness.Test.Services.SaleService;

public class CreateSaleAsyncTests
{
    // Test CreateSaleAsync method
    [Fact]
    public async Task CreateSaleAsync_ValidSale_CreatesSuccessfully()
    {
        // Arrange
        var mockSaleRepo = new Mock<ISaleRepository>();
        var mockProductRepo = new Mock<IProductRepository>();
        var mockSaleItemRepo = new Mock<ISaleItemRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockUow = new Mock<IUnitOfWork>();

        var service = new Application.Services.SaleService(
            mockSaleRepo.Object,
            mockProductRepo.Object,
            mockSaleItemRepo.Object,
            mockCustomerRepo.Object,
            mockUow.Object
        );

        var product = new Product("SKU001", "Test Product", "Desc", 50m, "img.jpg", 10);
        var lines = new List<(Guid, int)> { (product.Id, 2) };

        mockProductRepo.Setup(repo => repo.GetByIdAsync(product.Id))
            .ReturnsAsync(product);

        mockProductRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        mockUow.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await service.CreateSaleAsync(Guid.NewGuid(), lines);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(100m, result.TotalAmount);
        Assert.Equal(8, product.Stock);
    }

    // Test CreateSaleAsync method with empty lines
    [Fact]
    public async Task CreateSaleAsync_EmptyLines_ThrowsArgumentException()
    {
        // Arrange
        var mockSaleRepo = new Mock<ISaleRepository>();
        var mockProductRepo = new Mock<IProductRepository>();
        var mockSaleItemRepo = new Mock<ISaleItemRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockUow = new Mock<IUnitOfWork>();

        var service = new Application.Services.SaleService(
            mockSaleRepo.Object,
            mockProductRepo.Object,
            mockSaleItemRepo.Object,
            mockCustomerRepo.Object,
            mockUow.Object
        );

        var emptyLines = new List<(Guid, int)>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateSaleAsync(Guid.NewGuid(), emptyLines));
    }

    // Test CreateSaleAsync method with insufficient stock
    [Fact]
    public async Task CreateSaleAsync_InsufficientStock_ThrowsInvalidOperationException()
    {
        // Arrange
        var mockSaleRepo = new Mock<ISaleRepository>();
        var mockProductRepo = new Mock<IProductRepository>();
        var mockSaleItemRepo = new Mock<ISaleItemRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockUow = new Mock<IUnitOfWork>();

        var service = new Application.Services.SaleService(
            mockSaleRepo.Object,
            mockProductRepo.Object,
            mockSaleItemRepo.Object,
            mockCustomerRepo.Object,
            mockUow.Object
        );

        var product = new Product("SKU001", "Test Product", "Desc", 50m, "img.jpg", 5);
        var lines = new List<(Guid, int)> { (product.Id, 10) };

        mockProductRepo.Setup(repo => repo.GetByIdAsync(product.Id))
            .ReturnsAsync(product);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateSaleAsync(Guid.NewGuid(), lines));

        Assert.Contains("Stock is not enough", exception.Message);
    }
}
