using AutoMapper;
using Firmness.Application.Services;
using Firmness.Domain.Common;
using Firmness.Domain.DTOs.Vehicle;
using Firmness.Domain.Entities;
using Firmness.Domain.Enums;
using Firmness.Domain.Interfaces;
using Moq;

namespace Firmness.Test.Services.VehicleService;

// Test Create and Update operations
public class CreateUpdateVehicleTests
{
    private readonly Mock<IVehicleRepository> _mockVehicleRepo;
    private readonly Mock<IVehicleRentalRepository> _mockRentalRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Application.Services.VehicleService _service;

    public CreateUpdateVehicleTests()
    {
        _mockVehicleRepo = new Mock<IVehicleRepository>();
        _mockRentalRepo = new Mock<IVehicleRentalRepository>();
        _mockMapper = new Mock<IMapper>();

        _service = new Application.Services.VehicleService(
            _mockVehicleRepo.Object,
            _mockRentalRepo.Object,
            _mockMapper.Object
        );
    }

    // Test CreateVehicleAsync method
    [Fact]
    public async Task CreateVehicleAsync_ValidVehicle_ReturnsSuccess()
    {
        // Arrange
        var createDto = new CreateVehicleDto { LicensePlate = "ABC123", Brand = "Toyota", Model = "Camry" };
        var vehicle = new Vehicle("Toyota", "Camry", 2022, "ABC123", VehicleType.Truck);
        var vehicleDto = new VehicleDto { LicensePlate = "ABC123" };

        _mockVehicleRepo.Setup(repo => repo.GetByLicensePlateAsync(createDto.LicensePlate))
            .ReturnsAsync((Vehicle?)null);

        _mockMapper.Setup(m => m.Map<Vehicle>(createDto))
            .Returns(vehicle);

        _mockVehicleRepo.Setup(repo => repo.AddAsync(It.IsAny<Vehicle>()))
            .Returns(Task.CompletedTask);

        _mockMapper.Setup(m => m.Map<VehicleDto>(vehicle))
            .Returns(vehicleDto);

        // Act
        var result = await _service.CreateVehicleAsync(createDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    // Test CreateVehicleAsync method with duplicate license plate
    [Fact]
    public async Task CreateVehicleAsync_DuplicateLicensePlate_ReturnsFailure()
    {
        // Arrange
        var createDto = new CreateVehicleDto { LicensePlate = "ABC123" };
        var existingVehicle = new Vehicle("Toyota", "Camry", 2022, "ABC123", VehicleType.Truck);

        _mockVehicleRepo.Setup(repo => repo.GetByLicensePlateAsync(createDto.LicensePlate))
            .ReturnsAsync(existingVehicle);

        // Act
        var result = await _service.CreateVehicleAsync(createDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.Validation, result.ErrorCode);
    }
}
