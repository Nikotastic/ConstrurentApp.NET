using Firmness.Application.Interfaces;
using Firmness.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Firmness.Infrastructure.Payment;

// Payment simulator for development and testing
public class SimulatedPaymentService : IPaymentService
{
    private readonly ILogger<SimulatedPaymentService> _logger;

    public SimulatedPaymentService(ILogger<SimulatedPaymentService> logger)
    {
        _logger = logger;
    }

    public async Task<Result<PaymentResult>> ProcessPaymentAsync(
        PaymentRequest request, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "💳 Processing simulated payment: {Amount} {Currency} for {Customer}",
            request.Amount,
            request.Currency,
            request.CustomerEmail);

        // Simulate processing delay (like a real gateway)
        await Task.Delay(1500, cancellationToken);

        // Simulate failure if requested
        if (request.SimulateFailure)
        {
            _logger.LogWarning("❌ Simulated payment failed for {Customer}", request.CustomerEmail);
            return Result<PaymentResult>.Failure(
                "Payment declined by bank (simulated)",
                ErrorCodes.PaymentFailed);
        }

        // Simulate successful payment
        var paymentResult = new PaymentResult
        {
            TransactionId = $"SIM-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            Status = "Completed",
            Amount = request.Amount,
            ProcessedAt = DateTime.UtcNow,
            PaymentMethod = "Simulated Credit Card"
        };

        _logger.LogInformation(
            "✅ Simulated payment successful: {TransactionId} - {Amount} {Currency}",
            paymentResult.TransactionId,
            paymentResult.Amount,
            request.Currency);

        return Result<PaymentResult>.Success(paymentResult);
    }
}
