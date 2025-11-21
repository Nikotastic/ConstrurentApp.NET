using AutoMapper;
using Firmness.Application.Interfaces;
using Firmness.Application.Services;
using Firmness.Domain.Common;
using Firmness.Domain.DTOs.Vehicle;
using Firmness.Domain.Entities;
using Firmness.Domain.Enums;
using Firmness.Domain.Interfaces;
using Moq;

namespace Firmness.Test.Services.VehicleRentalService;

// Test CreateRentalAsync method
public class CreateRentalTests
{
    private readonly Mock<IVehicleRentalRepository> _mockRentalRepo;
    private readonly Mock<IVehicleRepository> _mockVehicleRepo;
    private readonly Mock<ICustomerRepository> _mockCustomerRepo;
    private readonly Mock<INotificationService> _mockNotificationService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Application.Services.VehicleRentalService _service;

    public CreateRentalTests()
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

    // Test CreateRentalAsync method
    [Fact]
    public async Task CreateRentalAsync_ValidData_ReturnsSuccess()
    {
        // Arrange
        var createDto = new CreateVehicleRentalDto 
        { 
            VehicleId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            StartDate = DateTime.Today.AddDays(1),
            EstimatedReturnDate = DateTime.Today.AddDays(5),
            RentalRate = 100m,
            RentalPeriodType = "Daily"
        };

        var vehicle = new Vehicle("Toyota", "Camry", 2022, "ABC", VehicleType.Truck)
        {
            Status = VehicleStatus.Available,
            DailyRate = 100
        };
        TestHelper.SetId(vehicle, createDto.VehicleId);
        
        var customer = new Customer("John", "Doe", "email@test.com");
        TestHelper.SetId(customer, createDto.CustomerId);
        
        var rentalDto = new VehicleRentalDto { Id = Guid.NewGuid() };

        _mockVehicleRepo.Setup(r => r.GetByIdAsync(createDto.VehicleId)).ReturnsAsync(vehicle);
        _mockCustomerRepo.Setup(r => r.GetByIdAsync(createDto.CustomerId)).ReturnsAsync(customer);
        
        // Mock availability check (assuming service checks repository for overlaps)
        _mockRentalRepo.Setup(r => r.GetByVehicleIdAsync(createDto.VehicleId))
            .ReturnsAsync(new List<VehicleRental>());

        _mockRentalRepo.Setup(r => r.AddAsync(It.IsAny<VehicleRental>())).Returns(Task.CompletedTask);
        _mockVehicleRepo.Setup(r => r.UpdateAsync(It.IsAny<Vehicle>())).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<VehicleRentalDto>(It.IsAny<VehicleRental>())).Returns(rentalDto);

        // Act
        var result = await _service.CreateRentalAsync(createDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        _mockRentalRepo.Verify(r => r.AddAsync(It.IsAny<VehicleRental>()), Times.Once);
        _mockVehicleRepo.Verify(r => r.UpdateAsync(It.IsAny<Vehicle>()), Times.Once);
    }

    // Test CreateRentalAsync method with vehicle not available
    [Fact]
    public async Task CreateRentalAsync_VehicleNotAvailable_ReturnsFailure()
    {
        // Arrange
        var createDto = new CreateVehicleRentalDto 
        { 
            VehicleId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid()
        };
        var vehicle = new Vehicle("Toyota", "Camry", 2022, "ABC", VehicleType.Truck) 
        { 
            Status = VehicleStatus.Maintenance 
        };
        TestHelper.SetId(vehicle, createDto.VehicleId);
        
        var customer = new Customer("John", "Doe", "email@test.com");
        TestHelper.SetId(customer, createDto.CustomerId);

        _mockCustomerRepo.Setup(r => r.GetByIdAsync(createDto.CustomerId)).ReturnsAsync(customer);
        _mockVehicleRepo.Setup(r => r.GetByIdAsync(createDto.VehicleId)).ReturnsAsync(vehicle);

        // Act
        var result = await _service.CreateRentalAsync(createDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.Validation, result.ErrorCode);
    }
}
