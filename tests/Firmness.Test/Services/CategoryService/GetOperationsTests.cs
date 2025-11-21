using Firmness.Application.Services;
using Firmness.Domain.Common;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Moq;

namespace Firmness.Test.Services.CategoryService;

// Test Get operations
public class GetOperationsTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepo;
    private readonly Application.Services.CategoryService _service;

    public GetOperationsTests()
    {
        _mockCategoryRepo = new Mock<ICategoryRepository>();
        _service = new Application.Services.CategoryService(_mockCategoryRepo.Object);
    }
    // Test GetByIdAsync method
    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsCategory()
    {
        // Arrange
        var category = new Category("Tools", "Construction tools");
        var id = category.Id;

        _mockCategoryRepo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(category);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Tools", result.Name);
    }

    // Test GetAllAsync method
    [Fact]
    public async Task GetAllAsync_ReturnsAllCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category("Tools", "Desc"),
            new Category("Materials", "Desc")
        };

        _mockCategoryRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(categories);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    // Test GetActiveAsync method
    [Fact]
    public async Task GetActiveAsync_ReturnsOnlyActiveCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category("Tools", "Desc") { IsActive = true }
        };

        _mockCategoryRepo.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(categories);

        // Act
        var result = await _service.GetActiveAsync();

        // Assert
        Assert.Single(result);
        Assert.True(result.First().IsActive);
    }
}
