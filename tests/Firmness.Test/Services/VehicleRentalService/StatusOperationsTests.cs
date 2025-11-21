using AutoMapper;
using Firmness.Domain.DTOs.Vehicle;
using Firmness.Domain.Entities;
using Firmness.Domain.Enums;
using Firmness.Domain.Interfaces;
using Firmness.Application.Interfaces;
using Moq;

namespace Firmness.Test.Services.VehicleRentalService;

// Test Status operations
public class StatusOperationsTests
{
    private readonly Mock<IVehicleRentalRepository> _mockRentalRepo;
    private readonly Mock<IVehicleRepository> _mockVehicleRepo;
    private readonly Mock<ICustomerRepository> _mockCustomerRepo;
    private readonly Mock<INotificationService> _mockNotificationService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Application.Services.VehicleRentalService _service;

    public StatusOperationsTests()
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

    // Test CancelRentalAsync method
    [Fact]
    public async Task CancelRentalAsync_ValidId_CancelsRental()
    {
        // Arrange
        var id = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var rental = new VehicleRental(Guid.NewGuid(), vehicleId, DateTime.Now, DateTime.Now.AddDays(1), 100);
        TestHelper.SetId(rental, id);
        rental.Status = RentalStatus.Pending;
        
        var vehicle = new Vehicle("Toyota", "Camry", 2022, "ABC", VehicleType.Truck) { Status = VehicleStatus.Rented };
        TestHelper.SetId(vehicle, vehicleId);

        _mockRentalRepo.Setup(r => r.GetByIdWithDetailsAsync(id)).ReturnsAsync(rental);
        _mockVehicleRepo.Setup(r => r.GetByIdAsync(vehicleId)).ReturnsAsync(vehicle);
        _mockRentalRepo.Setup(r => r.UpdateAsync(It.IsAny<VehicleRental>())).Returns(Task.CompletedTask);
        _mockVehicleRepo.Setup(r => r.UpdateAsync(It.IsAny<Vehicle>())).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<VehicleRentalDto>(It.IsAny<VehicleRental>())).Returns(new VehicleRentalDto());

        // Act
        var result = await _service.CancelRentalAsync(id, "Reason");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(RentalStatus.Cancelled, rental.Status);
        Assert.Equal(VehicleStatus.Available, vehicle.Status);
    }

    // Test CompleteRentalAsync method
    [Fact]
    public async Task CompleteRentalAsync_ValidId_CompletesRental()
    {
        // Arrange
        var id = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var completeDto = new CompleteVehicleRentalDto { ReturnDate = DateTime.Now, FinalMileage = 1000 };
        
        var vehicle = new Vehicle("Toyota", "Camry", 2022, "ABC", VehicleType.Truck) { Status = VehicleStatus.Rented };
        TestHelper.SetId(vehicle, vehicleId);
        
        var rental = new VehicleRental(Guid.NewGuid(), vehicleId, DateTime.Now, DateTime.Now.AddDays(1), 100);
        TestHelper.SetId(rental, id);
        rental.Status = RentalStatus.Active;
        // Set the Vehicle navigation property
        TestHelper.SetProperty(rental, "Vehicle", vehicle);

        _mockRentalRepo.Setup(r => r.GetByIdWithDetailsAsync(id)).ReturnsAsync(rental);
        _mockRentalRepo.Setup(r => r.UpdateAsync(It.IsAny<VehicleRental>())).Returns(Task.CompletedTask);
        _mockVehicleRepo.Setup(r => r.UpdateAsync(It.IsAny<Vehicle>())).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<VehicleRentalDto>(It.IsAny<VehicleRental>())).Returns(new VehicleRentalDto());

        // Act
        var result = await _service.CompleteRentalAsync(id, completeDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(RentalStatus.Completed, rental.Status);
        Assert.Equal(VehicleStatus.Available, vehicle.Status);
    }
}
