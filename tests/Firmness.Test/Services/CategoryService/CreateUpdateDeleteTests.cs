using Firmness.Application.Services;
using Firmness.Domain.Common;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Moq;

namespace Firmness.Test.Services.CategoryService;

// Test Create, Update and Delete operations
public class CreateUpdateDeleteTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepo;
    private readonly Application.Services.CategoryService _service;

    public CreateUpdateDeleteTests()
    {
        _mockCategoryRepo = new Mock<ICategoryRepository>();
        _service = new Application.Services.CategoryService(_mockCategoryRepo.Object);
    }

    // Test AddAsync method
    [Fact]
    public async Task AddAsync_ValidCategory_ReturnsSuccess()
    {
        // Arrange
        var category = new Category("Tools", "Desc");
        _mockCategoryRepo.Setup(r => r.AddAsync(category, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.AddAsync(category);

        // Assert
        Assert.True(result.IsSuccess);
        _mockCategoryRepo.Verify(r => r.AddAsync(category, It.IsAny<CancellationToken>()), Times.Once);
    }

    // Test AddAsync method with null category
    [Fact]
    public async Task AddAsync_NullCategory_ReturnsFailure()
    {
        // Act
        var result = await _service.AddAsync(null!);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.Validation, result.ErrorCode);
    }

    // Test UpdateAsync method
    [Fact]
    public async Task UpdateAsync_ValidCategory_ReturnsSuccess()
    {
        // Arrange
        var category = new Category("Tools", "Desc");
        TestHelper.SetId(category, Guid.NewGuid());
        _mockCategoryRepo.Setup(r => r.ExistsAsync(category.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockCategoryRepo.Setup(r => r.UpdateAsync(category, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateAsync(category);

        // Assert
        Assert.True(result.IsSuccess);
        _mockCategoryRepo.Verify(r => r.UpdateAsync(category, It.IsAny<CancellationToken>()), Times.Once);
    }

    // Test DeleteAsync method
    [Fact]
    public async Task DeleteAsync_ValidId_ReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockCategoryRepo.Setup(r => r.ExistsAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockCategoryRepo.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        Assert.True(result.IsSuccess);
        _mockCategoryRepo.Verify(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
