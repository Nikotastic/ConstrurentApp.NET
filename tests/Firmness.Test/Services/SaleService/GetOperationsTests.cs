using Firmness.Application.Services;
using Firmness.Domain.Common;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Moq;

namespace Firmness.Test.Services.SaleService;

// Test Get operations
public class GetOperationsTests
{
    private Mock<ISaleRepository> _mockSaleRepo;
    private Mock<IProductRepository> _mockProductRepo;
    private Mock<ISaleItemRepository> _mockSaleItemRepo;
    private Mock<ICustomerRepository> _mockCustomerRepo;
    private Mock<IUnitOfWork> _mockUow;
    private Application.Services.SaleService _service;

    public GetOperationsTests()
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

    // Test GetByIdAsync method
    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsSale()
    {
        // Arrange
        var sale = new Sale(Guid.NewGuid());
        var saleId = sale.Id;

        _mockSaleRepo.Setup(repo => repo.GetByIdAsync(saleId))
            .ReturnsAsync(sale);

        // Act
        var result = await _service.GetByIdAsync(saleId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(saleId, result.Id);
    }

    // Test GetAllAsync method
    [Fact]
    public async Task GetAllAsync_ReturnsAllSales()
    {
        // Arrange
        var sales = new List<Sale>
        {
            new Sale(Guid.NewGuid()),
            new Sale(Guid.NewGuid())
        };

        _mockSaleRepo.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(sales);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    // Test GetByCustomerIdAsync method
    [Fact]
    public async Task GetByCustomerIdAsync_ValidCustomerId_ReturnsCustomerSales()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var sales = new List<Sale>
        {
            new Sale(customerId),
            new Sale(customerId)
        };

        _mockSaleRepo.Setup(repo => repo.GetByCustomerIdAsync(customerId))
            .ReturnsAsync(sales);

        // Act
        var result = await _service.GetByCustomerIdAsync(customerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
}
