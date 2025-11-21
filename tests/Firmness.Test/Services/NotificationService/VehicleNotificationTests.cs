using Firmness.Application.Interfaces;
using Firmness.Application.Services;
using Firmness.Domain.Entities;
using Firmness.Test.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace Firmness.Test.Services.NotificationService;

public class VehicleNotificationTests
{
    // Test Send VehicleRental ConfirmationAsync method
    [Fact]
    public async Task SendVehicleRentalConfirmationAsync_ValidData_SendsEmail()
    {
        // Arrange
        var mockEmailService = new MockEmailService();
        var mockLogger = new Mock<ILogger<Application.Services.NotificationService>>();
        var service = new Application.Services.NotificationService(mockEmailService, mockLogger.Object);

        var customer = new Customer("John", "Doe", "john@example.com")
        {
            Phone = "1234567890",
            Address = "Address"
        };
        var vehicleName = "Toyota Camry - ABC123";
        var startDate = DateTime.Now;
        var endDate = DateTime.Now.AddDays(7);
        var totalAmount = 350m;

        // Act
        await service.SendVehicleRentalConfirmationAsync(customer, vehicleName, startDate, endDate, totalAmount);

        // Assert
        Assert.Single(mockEmailService.SentEmails);
        Assert.Equal("john@example.com", mockEmailService.SentEmails[0].To);
        Assert.Contains("Confirmation", mockEmailService.SentEmails[0].Subject);
    }

    // Test Send VehicleReturn ReminderAsync method
    [Fact]
    public async Task SendVehicleReturnReminderAsync_ValidData_SendsEmail()
    {
        // Arrange
        var mockEmailService = new MockEmailService();
        var mockLogger = new Mock<ILogger<Application.Services.NotificationService>>();
        var service = new Application.Services.NotificationService(mockEmailService, mockLogger.Object);

        var customer = new Customer("John", "Doe", "john@example.com")
        {
            Phone = "1234567890",
            Address = "Address"
        };
        var vehicleName = "Toyota Camry";
        var returnDate = DateTime.Now.AddDays(1);

        // Act
        await service.SendVehicleReturnReminderAsync(customer, vehicleName, returnDate);

        // Assert
        Assert.Single(mockEmailService.SentEmails);
        Assert.Contains("Reminder", mockEmailService.SentEmails[0].Subject);
    }
}
