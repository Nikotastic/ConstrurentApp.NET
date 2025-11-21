using Firmness.Application.Services;
using Firmness.Domain.Entities;
using Firmness.Domain.Enums;
using Firmness.Domain.Interfaces;
using Moq;

namespace Firmness.Test.Services.DashboardService;

// Test DashboardData method
public class DashboardDataTests
{
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly Mock<ICustomerRepository> _mockCustomerRepo;
    private readonly Mock<ISaleRepository> _mockSaleRepo;
    private readonly Application.Services.DashboardService _service;

    public DashboardDataTests()
    {
        _mockProductRepo = new Mock<IProductRepository>();
        _mockCustomerRepo = new Mock<ICustomerRepository>();
        _mockSaleRepo = new Mock<ISaleRepository>();

        _service = new Application.Services.DashboardService(
            _mockProductRepo.Object,
            _mockCustomerRepo.Object,
            _mockSaleRepo.Object
        );
    }

    // Test GetDashboardDataAsync method
    [Fact]
    public async Task GetDashboardDataAsync_ReturnsAggregatedData()
    {
        // Arrange
        _mockProductRepo.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(100);
        _mockCustomerRepo.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(50);
        _mockSaleRepo.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(200);
        
        var sales = new List<Sale>
        {
            new Sale(Guid.NewGuid()) { TotalAmount = 500m, Status = SaleStatus.Completed },
            new Sale(Guid.NewGuid()) { TotalAmount = 500m, Status = SaleStatus.Completed }
        };
        
        _mockSaleRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(sales);

        // Act
        var result = await _service.GetDashboardDataAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100, result.TotalProducts);
        Assert.Equal(50, result.TotalCustomers);
        Assert.Equal(200, result.TotalSales);
        Assert.Equal(1000m, result.TotalRevenue);
    }

    // Test GetTotalProductsAsync method
    [Fact]
    public async Task GetTotalProductsAsync_ReturnsCount()
    {
        // Arrange
        _mockProductRepo.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(10);

        // Act
        var result = await _service.GetTotalProductsAsync();

        // Assert
        Assert.Equal(10, result);
    }
}
