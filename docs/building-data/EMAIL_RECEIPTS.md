# 📧 Email Receipt Implementation Guide

## Overview

This guide shows how to send purchase receipts via email with PDF attachments after a successful payment.

## Prerequisites

- ✅ Email service configured (Gmail SMTP)
- ✅ QuestPDF installed for PDF generation
- ✅ EmailMessage entity updated with attachment support

## Implementation Steps

### 1. Generate the PDF Receipt

```csharp
// In your service or controller
using QuestPDF.Fluent;

public byte[] GenerateReceiptPdf(Sale sale, Customer customer)
{
    var document = new ReceiptDocument(sale, customer);
    return document.GeneratePdf();
}
```

### 2. Send Email with Attachment

```csharp
// After successful payment/purchase
var pdfBytes = GenerateReceiptPdf(sale, customer);

var emailMessage = EmailMessage.CreateReceiptEmail(
    customerEmail: customer.Email,
    customerName: customer.FullName,
    totalAmount: sale.TotalAmount,
    invoiceNumber: sale.InvoiceNumber,
    pdfContent: pdfBytes
);

await _emailService.SendEmailAsync(emailMessage);
```

### 3. Complete Example in Controller

```csharp
[HttpPost("sales/checkout")]
public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
{
    // 1. Process payment
    var paymentResult = await _paymentService.ProcessPaymentAsync(request);

    if (!paymentResult.IsSuccess)
        return BadRequest(paymentResult.Error);

    // 2. Create sale record
    var sale = await _salesService.CreateSaleAsync(request);

    // 3. Generate PDF receipt
    var customer = await _customerService.GetByIdAsync(request.CustomerId);
    var pdfBytes = GenerateReceiptPdf(sale, customer);

    // 4. Send email with PDF attachment
    var emailMessage = EmailMessage.CreateReceiptEmail(
        customer.Email,
        customer.FullName,
        sale.TotalAmount,
        sale.InvoiceNumber,
        pdfBytes
    );

    try
    {
        await _emailService.SendEmailAsync(emailMessage);

        return Ok(new
        {
            message = "Purchase completed successfully. Receipt sent to your email.",
            saleId = sale.Id,
            invoiceNumber = sale.InvoiceNumber
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to send receipt email");

        // Still return success since payment was processed
        return Ok(new
        {
            message = "Purchase completed. Email delivery failed, please contact support.",
            saleId = sale.Id
        });
    }
}
```

### 4. Frontend Integration (Angular)

```typescript
// In your checkout component
checkout() {
  this.salesService.checkout(this.checkoutData).subscribe({
    next: (response) => {
      this.showSuccess('Purchase completed! Receipt sent to your email.');
      this.router.navigate(['/orders', response.saleId]);
    },
    error: (error) => {
      this.showError('Payment failed. Please try again.');
    }
  });
}
```

## Testing

### Test Email Configuration

Ensure your `.env` or `appsettings.json` has:

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderName": "Firmness Team",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "EnableSsl": true,
    "TimeoutSeconds": 30
  }
}
```

### Gmail App Password

For Gmail, you need to create an **App Password**:

1. Go to Google Account Settings
2. Security → 2-Step Verification
3. App Passwords → Generate new password
4. Use this password in your configuration

## Troubleshooting

| Issue             | Solution                                       |
| ----------------- | ---------------------------------------------- |
| Email not sending | Check logs for SMTP errors, verify credentials |
| PDF not attached  | Ensure `pdfBytes` is not null/empty            |
| Timeout errors    | Increase `TimeoutSeconds` in settings          |
