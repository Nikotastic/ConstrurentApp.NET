using Firmness.Application.Interfaces;
using Firmness.Domain.Common;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Firmness.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentTestController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IPdfService _pdfService;
    private readonly IEmailService _emailService;
    private readonly ILogger<PaymentTestController> _logger;

    public PaymentTestController(
        IPaymentService paymentService,
        IPdfService pdfService,
        IEmailService emailService,
        ILogger<PaymentTestController> logger)
    {
        _paymentService = paymentService;
        _pdfService = pdfService;
        _emailService = emailService;
        _logger = logger;
    }

    // Simulate a complete purchase: Payment → PDF → Email
    [HttpPost("simulate-purchase")]
    public async Task<IActionResult> SimulatePurchase([FromBody] SimulatePurchaseRequest request)
    {
        try
        {
            _logger.LogInformation("🛒 Starting simulated purchase for {Customer}", request.CustomerEmail);

            // 1. Process simulated payment
            var paymentRequest = new PaymentRequest
            {
                Amount = request.TotalAmount,
                Currency = "USD",
                CustomerEmail = request.CustomerEmail,
                CustomerName = request.CustomerName,
                Description = "Test Purchase",
                SimulateFailure = request.SimulateFailure
            };

            var paymentResult = await _paymentService.ProcessPaymentAsync(paymentRequest);

            if (!paymentResult.IsSuccess)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Payment failed",
                    error = paymentResult.ErrorMessage
                });
            }

            // 2. Generate invoice number
            var invoiceNumber = $"INV-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

            // 3. Generate receipt PDF
            var receiptData = new ReceiptData
            {
                InvoiceNumber = invoiceNumber,
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                Date = DateTime.Now,
                TotalAmount = request.TotalAmount,
                TransactionId = paymentResult.Value.TransactionId,
                Items = request.Items.Select(i => new ReceiptItem
                {
                    Description = i.Description,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            var pdfBytes = _pdfService.GenerateReceiptPdf(receiptData);

            _logger.LogInformation("📄 PDF receipt generated: {Size} bytes", pdfBytes.Length);

            // 4. Send email with PDF attachment
            var emailMessage = EmailMessage.CreateReceiptEmail(
                request.CustomerEmail,
                request.CustomerName,
                request.TotalAmount,
                invoiceNumber,
                pdfBytes
            );

            await _emailService.SendEmailAsync(emailMessage);

            _logger.LogInformation("✅ Purchase simulation completed successfully");

            return Ok(new
            {
                success = true,
                message = "Purchase completed successfully! Receipt sent to your email.",
                data = new
                {
                    invoiceNumber,
                    transactionId = paymentResult.Value.TransactionId,
                    amount = request.TotalAmount,
                    processedAt = paymentResult.Value.ProcessedAt,
                    emailSent = true
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error during purchase simulation");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred during purchase",
                error = ex.Message
            });
        }
    }

    // Only generates and downloads the PDF (without sending email)
    [HttpPost("generate-receipt-pdf")]
    public IActionResult GenerateReceiptPdf([FromBody] SimulatePurchaseRequest request)
    {
        var receiptData = new ReceiptData
        {
            InvoiceNumber = $"INV-{DateTime.Now:yyyyMMdd}-TEST",
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            Date = DateTime.Now,
            TotalAmount = request.TotalAmount,
            TransactionId = "TEST-TRANSACTION",
            Items = request.Items.Select(i => new ReceiptItem
            {
                Description = i.Description,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        var pdfBytes = _pdfService.GenerateReceiptPdf(receiptData);

        return File(pdfBytes, "application/pdf", $"Receipt_{receiptData.InvoiceNumber}.pdf");
    }
}

public class SimulatePurchaseRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public List<PurchaseItem> Items { get; set; } = new();
    public bool SimulateFailure { get; set; } = false;
}

public class PurchaseItem
{
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
