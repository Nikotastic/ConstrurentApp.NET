using AutoMapper;
using Firmness.Application.Services;
using Firmness.Domain.Common;
using Firmness.Domain.DTOs.Vehicle;
using Firmness.Domain.Entities;
using Firmness.Domain.Enums;
using Firmness.Domain.Interfaces;
using Moq;

namespace Firmness.Test.Services.VehicleService;

// Test GetVehiclesAsync method
public class GetVehiclesTests
{
    private readonly Mock<IVehicleRepository> _mockVehicleRepo;
    private readonly Mock<IVehicleRentalRepository> _mockRentalRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Application.Services.VehicleService _service;

    public GetVehiclesTests()
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

    // Test GetAllVehiclesAsync method
    [Fact]
    public async Task GetAllVehiclesAsync_ReturnsSuccessWithVehicles()
    {
        // Arrange
        var vehicles = new List<Vehicle>
        {
            new Vehicle("Toyota", "Camry", 2022, "ABC123", VehicleType.Truck),
            new Vehicle("Honda", "Civic", 2021, "XYZ789", VehicleType.Truck)
        };

        var vehicleDtos = new List<VehicleDto>
        {
            new VehicleDto { LicensePlate = "ABC123" },
            new VehicleDto { LicensePlate = "XYZ789" }
        };

        _mockVehicleRepo.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(vehicles);

        _mockMapper.Setup(m => m.Map<IEnumerable<VehicleDto>>(vehicles))
            .Returns(vehicleDtos);

        // Act
        var result = await _service.GetAllVehiclesAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
    }

    // Test GetAvailableVehiclesAsync method
    [Fact]
    public async Task GetAvailableVehiclesAsync_ReturnsOnlyAvailableVehicles()
    {
        // Arrange
        var availableVehicles = new List<Vehicle>
        {
            new Vehicle("Toyota", "Camry", 2022, "ABC123", VehicleType.Truck) { Status = VehicleStatus.Available }
        };

        var vehicleDtos = new List<VehicleDto>
        {
            new VehicleDto { LicensePlate = "ABC123" }
        };

        _mockVehicleRepo.Setup(repo => repo.GetAvailableVehiclesAsync())
            .ReturnsAsync(availableVehicles);

        _mockMapper.Setup(m => m.Map<IEnumerable<VehicleDto>>(availableVehicles))
            .Returns(vehicleDtos);

        // Act
        var result = await _service.GetAvailableVehiclesAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
    }
}
