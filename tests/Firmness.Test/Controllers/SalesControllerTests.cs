using AutoMapper;
using Firmness.Api.Controllers;
using Firmness.Application.Interfaces;
using Firmness.Domain.Common;
using Firmness.Application.DTOs;
using Firmness.Domain.Entities;
using Firmness.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Firmness.Domain.Interfaces;

namespace Firmness.Test;

public class SalesControllerTests
{
    private readonly Mock<ISaleService> _mockSaleService;
    private readonly Mock<ICustomerService> _mockCustomerService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<ILogger<SalesController>> _mockLogger;
    private readonly SalesController _controller;

    public SalesControllerTests()
    {
        _mockSaleService = new Mock<ISaleService>();
        _mockCustomerService = new Mock<ICustomerService>();
        _mockMapper = new Mock<IMapper>();
        _mockEmailService = new Mock<IEmailService>();
        _mockLogger = new Mock<ILogger<SalesController>>();

        _controller = new SalesController(
            _mockSaleService.Object,
            _mockCustomerService.Object,
            _mockEmailService.Object,
            _mockMapper.Object,
            _mockLogger.Object
        );

        // Setup default HTTP context with a user
        SetupControllerContext();
    }

    private void SetupControllerContext(bool isAdmin = false, string userId = "user123")
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, "testuser")
        };

        if (isAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            }
        };
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ReturnsOkWithSales()
    {
        // Arrange
        SetupControllerContext(isAdmin: true);
        
        var sales = new List<Sale>
        {
            CreateSampleSale(),
            CreateSampleSale()
        };
        
        var saleDtos = new List<SaleDto>
        {
            new SaleDto { Id = sales[0].Id, TotalAmount = 100 },
            new SaleDto { Id = sales[1].Id, TotalAmount = 200 }
        };

        _mockSaleService.Setup(s => s.GetAllWithDetailsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sales);
        
        _mockMapper.Setup(m => m.Map<IEnumerable<SaleDto>>(sales))
            .Returns(saleDtos);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        
        _mockSaleService.Verify(s => s.GetAllWithDetailsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAll_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        SetupControllerContext(isAdmin: true);
        
        _mockSaleService.Setup(s => s.GetAllWithDetailsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAll();

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_ValidId_ReturnsOkWithSale()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateSampleSale(saleId);
        var saleDto = new SaleDto { Id = saleId, TotalAmount = 100 };

        _mockSaleService.Setup(s => s.GetByIdWithDetailsAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        
        _mockMapper.Setup(m => m.Map<SaleDto>(sale))
            .Returns(saleDto);

        // Act
        var result = await _controller.GetById(saleId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        
        _mockSaleService.Verify(s => s.GetByIdWithDetailsAsync(saleId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        
        _mockSaleService.Setup(s => s.GetByIdWithDetailsAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale?)null);

        // Act
        var result = await _controller.GetById(saleId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetById_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        
        _mockSaleService.Setup(s => s.GetByIdWithDetailsAsync(saleId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetById(saleId);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region GetByCustomerId Tests

    [Fact]
    public async Task GetByCustomerId_AdminUser_ReturnsOkWithSales()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        SetupControllerContext(isAdmin: true);
        
        var sales = new List<Sale> { CreateSampleSale() };
        var saleDtos = new List<SaleDto> { new SaleDto { Id = sales[0].Id } };

        _mockSaleService.Setup(s => s.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sales);
        
        _mockMapper.Setup(m => m.Map<IEnumerable<SaleDto>>(sales))
            .Returns(saleDtos);

        // Act
        var result = await _controller.GetByCustomerId(customerId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetByCustomerId_NonAdminUserAccessingOwnSales_ReturnsOk()
    {
        // Arrange
        var userId = "user123";
        SetupControllerContext(isAdmin: false, userId: userId);
        
        var customer = new Customer("John", "Doe", "john@example.com")
        {
            IdentityUserId = userId
        };
        var customerId = customer.Id; // Use the generated Id

        _mockCustomerService.Setup(s => s.GetByIdentityUserIdAsync(userId))
            .ReturnsAsync(customer);

        var sales = new List<Sale> { CreateSampleSale() };
        var saleDtos = new List<SaleDto> { new SaleDto { Id = sales[0].Id } };

        _mockSaleService.Setup(s => s.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sales);
        
        _mockMapper.Setup(m => m.Map<IEnumerable<SaleDto>>(sales))
            .Returns(saleDtos);

        // Act
        var result = await _controller.GetByCustomerId(customerId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetByCustomerId_NonAdminUserAccessingOthersSales_ReturnsForbid()
    {
        // Arrange
        var userId = "user123";
        SetupControllerContext(isAdmin: false, userId: userId);
        
        // Current user's customer
        var currentCustomer = new Customer("John", "Doe", "john@example.com")
        {
            IdentityUserId = userId
        };
        
        // Another customer's ID that the user is trying to access
        var otherCustomerId = Guid.NewGuid();

        _mockCustomerService.Setup(s => s.GetByIdentityUserIdAsync(userId))
            .ReturnsAsync(currentCustomer);

        // Act
        var result = await _controller.GetByCustomerId(otherCustomerId);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_ValidDto_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateSaleDto
        {
            CustomerId = Guid.NewGuid(),
            PaymentMethod = PaymentMethod.Cash,
            Items = new List<CreateSaleItemDto>
            {
                new CreateSaleItemDto { ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 50 }
            }
        };

        var sale = CreateSampleSale();
        var saleDto = new SaleDto { Id = sale.Id, TotalAmount = 100 };

        _mockSaleService.Setup(s => s.CreateSaleAsync(
                It.IsAny<Guid>(), 
                It.IsAny<IEnumerable<(Guid, int)>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        _mockSaleService.Setup(s => s.UpdateAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockSaleService.Setup(s => s.GetByIdWithDetailsAsync(sale.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        _mockMapper.Setup(m => m.Map<SaleDto>(sale))
            .Returns(saleDto);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(SalesController.GetById), createdResult.ActionName);
        
        _mockSaleService.Verify(s => s.CreateSaleAsync(
            It.IsAny<Guid>(), 
            It.IsAny<IEnumerable<(Guid, int)>>(), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_DtoWithNoItems_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateSaleDto
        {
            CustomerId = Guid.NewGuid(),
            PaymentMethod = PaymentMethod.Cash,
            Items = new List<CreateSaleItemDto>() // Empty list
        };

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }

    [Fact]
    public async Task Create_DtoWithNullItems_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateSaleDto
        {
            CustomerId = Guid.NewGuid(),
            PaymentMethod = PaymentMethod.Cash,
            Items = null! // Null items
        };

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }

    [Fact]
    public async Task Create_ServiceThrowsInvalidOperationException_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateSaleDto
        {
            CustomerId = Guid.NewGuid(),
            PaymentMethod = PaymentMethod.Cash,
            Items = new List<CreateSaleItemDto>
            {
                new CreateSaleItemDto { ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 50 }
            }
        };

        _mockSaleService.Setup(s => s.CreateSaleAsync(
                It.IsAny<Guid>(), 
                It.IsAny<IEnumerable<(Guid, int)>>(), 
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Insufficient stock"));

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_ValidDto_ReturnsOk()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var updateDto = new UpdateSaleDto
        {
            Status = "Completed",
            Discount = 10
        };

        var sale = CreateSampleSale(saleId);
        var saleDto = new SaleDto { Id = saleId };

        _mockSaleService.Setup(s => s.GetByIdWithDetailsAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        _mockMapper.Setup(m => m.Map(updateDto, sale))
            .Returns(sale);

        _mockSaleService.Setup(s => s.UpdateAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockMapper.Setup(m => m.Map<SaleDto>(sale))
            .Returns(saleDto);

        // Act
        var result = await _controller.Update(saleId, updateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        
        _mockSaleService.Verify(s => s.UpdateAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var updateDto = new UpdateSaleDto { Status = "Completed" };

        _mockSaleService.Setup(s => s.GetByIdWithDetailsAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale?)null);

        // Act
        var result = await _controller.Update(saleId, updateDto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_ValidId_ReturnsNoContent()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateSampleSale(saleId);

        _mockSaleService.Setup(s => s.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        _mockSaleService.Setup(s => s.DeleteAsync(saleId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(saleId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockSaleService.Verify(s => s.DeleteAsync(saleId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var saleId = Guid.NewGuid();

        _mockSaleService.Setup(s => s.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale?)null);

        // Act
        var result = await _controller.Delete(saleId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    #endregion

    #region Count Tests

    [Fact]
    public async Task Count_Success_ReturnsOk()
    {
        // Arrange
        var expectedCount = 42L;
        var successResult = Result<long>.Success(expectedCount);

        _mockSaleService.Setup(s => s.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.Count();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Count_Failure_ReturnsBadRequest()
    {
        // Arrange
        var failureResult = Result<long>.Failure("Database error");

        _mockSaleService.Setup(s => s.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _controller.Count();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }

    #endregion

    #region Helper Methods

    private Sale CreateSampleSale(Guid? saleId = null)
    {
        var customerId = Guid.NewGuid();

        var sale = new Sale(customerId)
        {
            TotalAmount = 100,
            Subtotal = 100,
            Tax = 0,
            Discount = 0,
            PaymentMethod = PaymentMethod.Cash
        };

        return sale;
    }

    #endregion
}
