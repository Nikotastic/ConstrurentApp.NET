using Firmness.Domain.Common;

namespace Firmness.Application.Interfaces;

public interface IPaymentService
{
    Task<Result<PaymentResult>> ProcessPaymentAsync(PaymentRequest request, CancellationToken cancellationToken = default);
}

public class PaymentRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Simulation: Use this field to force errors in testing
    public bool SimulateFailure { get; set; } = false;
}

public class PaymentResult
{
    public string TransactionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string PaymentMethod { get; set; } = "Simulated";
}
