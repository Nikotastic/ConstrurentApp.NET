using Firmness.Application.Services;
using Firmness.Domain.Common;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Moq;

namespace Firmness.Test.Services.SaleService;

// Test Update, Delete and Count operations
public class UpdateDeleteCountTests
{
    private Mock<ISaleRepository> _mockSaleRepo;
    private Mock<IProductRepository> _mockProductRepo;
    private Mock<ISaleItemRepository> _mockSaleItemRepo;
    private Mock<ICustomerRepository> _mockCustomerRepo;
    private Mock<IUnitOfWork> _mockUow;
    private Application.Services.SaleService _service;

    public UpdateDeleteCountTests()
    {
        _mockSaleRepo = new Mock<ISaleRepository>();
        _mockProductRepo = new Mock<IProductRepository>();
        _mockSaleItemRepo = new Mock<ISaleItemRepository>();
        _mockCustomerRepo = new Mock<ICustomerRepository>();
        _mockUow = new Mock<IUnitOfWork>();

        _service = new Application.Services.SaleService(
            _mockSaleRepo.Object,
            _mockProductRepo.Object,
            _mockSaleItemRepo.Object,
            _mockCustomerRepo.Object,
            _mockUow.Object
        );
    }

    // Test UpdateAsync method
    [Fact]
    public async Task UpdateAsync_ValidSale_UpdatesSuccessfully()
    {
        // Arrange
        var sale = new Sale(Guid.NewGuid());

        _mockSaleRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Sale>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(sale);

        // Assert
        _mockSaleRepo.Verify(repo => repo.UpdateAsync(sale), Times.Once);
    }

    // Test UpdateAsync method with null sale
    [Fact]
    public async Task UpdateAsync_NullSale_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.UpdateAsync(null!));
    }

    // Test DeleteAsync method
    [Fact]
    public async Task DeleteAsync_ValidId_DeletesSuccessfully()
    {
        // Arrange
        var saleId = Guid.NewGuid();

        _mockSaleRepo.Setup(repo => repo.DeleteAsync(saleId))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(saleId);

        // Assert
        _mockSaleRepo.Verify(repo => repo.DeleteAsync(saleId), Times.Once);
    }

    // Test CountAsync method
    [Fact]
    public async Task CountAsync_Success_ReturnsCount()
    {
        // Arrange
        var expectedCount = 25L;
        _mockSaleRepo.Setup(repo => repo.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _service.CountAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedCount, result.Value);
    }
}
