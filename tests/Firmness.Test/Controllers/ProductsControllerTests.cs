using AutoMapper;
using Firmness.Api.Controllers;
using Firmness.Application.Interfaces;
using Firmness.Domain.Common;
using Firmness.Application.DTOs;
using Firmness.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Firmness.Test.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<ProductsController>> _mockLogger;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<ProductsController>>();
        
        _controller = new ProductsController(
            _mockProductService.Object,
            _mockMapper.Object,
            _mockLogger.Object
        );
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ValidRequest_ReturnsOkWithProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product("SKU001", "Product 1", "Desc1", 100m, "img1.jpg", 10),
            new Product("SKU002", "Product 2", "Desc2", 200m, "img2.jpg", 20)
        };

        var paginatedResult = new PaginatedResult<Product>(products, 1, 50, 2);
        var result = Result<PaginatedResult<Product>>.Success(paginatedResult);

        _mockProductService.Setup(s => s.GetAllAsync(1, 50))
            .ReturnsAsync(result);

        var productDtos = new List<ProductDto>
        {
            new ProductDto { Id = products[0].Id, Name = "Product 1" },
            new ProductDto { Id = products[1].Id, Name = "Product 2" }
        };

        _mockMapper.Setup(m => m.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<Product>>()))
            .Returns(productDtos);

        // Act
        var actionResult = await _controller.GetAll(1, 50);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.NotNull(okResult.Value);
        _mockProductService.Verify(s => s.GetAllAsync(1, 50), Times.Once);
    }

    [Fact]
    public async Task GetAll_ServiceFailure_ReturnsBadRequest()
    {
        // Arrange
        var failure = Result<PaginatedResult<Product>>.Failure("Error", ErrorCodes.ServerError);
        _mockProductService.Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(failure);

        // Act
        var actionResult = await _controller.GetAll();

        // Assert
        Assert.IsAssignableFrom<IActionResult>(actionResult);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_ValidId_ReturnsOkWithProduct()
    {
        // Arrange
        var product = new Product("SKU001", "Test Product", "Description", 99.99m, "image.jpg", 10);
        var productId = product.Id;
        var result = Result<Product>.Success(product);

        _mockProductService.Setup(s => s.GetByIdAsync(productId))
            .ReturnsAsync(result);

        var productDto = new ProductDto { Id = productId, Name = "Test Product" };
        _mockMapper.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
            .Returns(productDto);

        // Act
        var actionResult = await _controller.GetById(productId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var result = Result<Product>.Failure("Not found", ErrorCodes.NotFound);

        _mockProductService.Setup(s => s.GetByIdAsync(productId))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetById(productId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(actionResult);
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_ValidProduct_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            SKU = "SKU001",
            Name = "New Product",
            Description = "Description",
            Price = 50m,
            Stock = 10
        };

        var product = new Product("SKU001", "New Product", "Description", 50m, "img.jpg", 10);
        
        _mockMapper.Setup(m => m.Map<Product>(It.IsAny<CreateProductDto>()))
            .Returns(product);

        _mockProductService.Setup(s => s.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync(Result.Success());

        var productDto = new ProductDto { Id = product.Id, Name = "New Product" };
        _mockMapper.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
            .Returns(productDto);

        // Act
        var actionResult = await _controller.Create(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        Assert.Equal(nameof(ProductsController.GetById), createdResult.ActionName);
        _mockProductService.Verify(s => s.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task Create_ServiceFailure_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateProductDto { Name = "Product", Price = 10m };
        var product = new Product("SKU", "Product", "Desc", 10m, "img.jpg", 5);

        _mockMapper.Setup(m => m.Map<Product>(It.IsAny<CreateProductDto>()))
            .Returns(product);

        _mockProductService.Setup(s => s.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync(Result.Failure("Validation error", ErrorCodes.Validation));

        // Act
        var actionResult = await _controller.Create(createDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(actionResult);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_ValidProduct_ReturnsOk()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var updateDto = new UpdateProductDto { Name = "Updated Product", Price = 75m };
        var existingProduct = new Product("SKU001", "Product", "Desc", 50m, "img.jpg", 10);

        _mockProductService.Setup(s => s.GetByIdAsync(productId))
            .ReturnsAsync(Result<Product>.Success(existingProduct));

        _mockMapper.Setup(m => m.Map(It.IsAny<UpdateProductDto>(), It.IsAny<Product>()))
            .Returns(existingProduct);

        _mockProductService.Setup(s => s.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync(Result.Success());

        var productDto = new ProductDto { Id = productId, Name = "Updated Product" };
        _mockMapper.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
            .Returns(productDto);

        // Act
        var actionResult = await _controller.Update(productId, updateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Update_ProductNotFound_ReturnsNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var updateDto = new UpdateProductDto { Name = "Updated" };

        _mockProductService.Setup(s => s.GetByIdAsync(productId))
            .ReturnsAsync(Result<Product>.Failure("Not found", ErrorCodes.NotFound));

        // Act
        var actionResult = await _controller.Update(productId, updateDto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(actionResult);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_ValidId_ReturnsNoContent()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _mockProductService.Setup(s => s.DeleteAsync(productId))
            .ReturnsAsync(Result.Success());

        // Act
        var actionResult = await _controller.Delete(productId);

        // Assert
        Assert.IsType<NoContentResult>(actionResult);
        _mockProductService.Verify(s => s.DeleteAsync(productId), Times.Once);
    }

    [Fact]
    public async Task Delete_ProductNotFound_ReturnsNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _mockProductService.Setup(s => s.DeleteAsync(productId))
            .ReturnsAsync(Result.Failure("Not found", ErrorCodes.NotFound));

        // Act
        var actionResult = await _controller.Delete(productId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(actionResult);
    }

    #endregion

    #region Count Tests

    [Fact]
    public async Task Count_Success_ReturnsOkWithCount()
    {
        // Arrange
        var expectedCount = 42L;
        _mockProductService.Setup(s => s.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<long>.Success(expectedCount));

        // Act
        var actionResult = await _controller.Count();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.NotNull(okResult.Value);
    }

    #endregion
}
