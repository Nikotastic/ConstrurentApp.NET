using AutoMapper;
using Firmness.Application.Services;
using Firmness.Application.DTOs.Vehicle;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Firmness.Application.Interfaces;
using Moq;
using Firmness.Domain.Common;

namespace Firmness.Test.Services.VehicleRentalService;

// Test GetRentalsAsync method
public class GetRentalsTests
{
    private readonly Mock<IVehicleRentalRepository> _mockRentalRepo;
    private readonly Mock<IVehicleRepository> _mockVehicleRepo;
    private readonly Mock<ICustomerRepository> _mockCustomerRepo;
    private readonly Mock<INotificationService> _mockNotificationService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Application.Services.VehicleRentalService _service;

    public GetRentalsTests()
    {
        _mockRentalRepo = new Mock<IVehicleRentalRepository>();
        _mockVehicleRepo = new Mock<IVehicleRepository>();
        _mockCustomerRepo = new Mock<ICustomerRepository>();
        _mockNotificationService = new Mock<INotificationService>();
        _mockMapper = new Mock<IMapper>();

        _service = new Application.Services.VehicleRentalService(
            _mockRentalRepo.Object,
            _mockVehicleRepo.Object,
            _mockCustomerRepo.Object,
            _mockNotificationService.Object,
            _mockMapper.Object
        );
    }

    // Test GetAllRentalsAsync method
    [Fact]
    public async Task GetAllRentalsAsync_ReturnsSuccess()
    {
        // Arrange
        var rentals = new List<VehicleRental> 
        { 
            new VehicleRental(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, DateTime.Now.AddDays(1), 100),
            new VehicleRental(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, DateTime.Now.AddDays(1), 100)
        };
        var dtos = new List<VehicleRentalDto> { new VehicleRentalDto(), new VehicleRentalDto() };

        _mockRentalRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(rentals);
        _mockMapper.Setup(m => m.Map<IEnumerable<VehicleRentalDto>>(rentals)).Returns(dtos);

        // Act
        var result = await _service.GetAllRentalsAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
    }

    // Test GetRentalByIdAsync method
    [Fact]
    public async Task GetRentalByIdAsync_ValidId_ReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var rental = new VehicleRental(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, DateTime.Now.AddDays(1), 100);
        TestHelper.SetId(rental, id);
        
        var dto = new VehicleRentalDto { Id = id };

        _mockRentalRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(rental);
        _mockMapper.Setup(m => m.Map<VehicleRentalDto>(rental)).Returns(dto);

        // Act
        var result = await _service.GetRentalByIdAsync(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value!.Id);
    }

    // Test GetRentalByIdAsync method with invalid id
    [Fact]
    public async Task GetRentalByIdAsync_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRentalRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((VehicleRental?)null);

        // Act
        var result = await _service.GetRentalByIdAsync(id);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.NotFound, result.ErrorCode);
    }
}
