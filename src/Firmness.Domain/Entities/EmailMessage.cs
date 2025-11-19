namespace Firmness.Domain.Entities;

public class EmailMessage
{
     public string To { get; private set; }
    public string Subject { get; private set; }
    public string Body { get; private set; }
    public bool IsHtml { get; private set; }
    public List<string>? CcRecipients { get; private set; }
    public List<string>? BccRecipients { get; private set; }

    public EmailMessage(
        string to,
        string subject,
        string body,
        bool isHtml = true,
        List<string>? ccRecipients = null,
        List<string>? bccRecipients = null)
    {
        if (string.IsNullOrWhiteSpace(to))
            throw new ArgumentException("Recipient email is required", nameof(to));
        
        if (string.IsNullOrWhiteSpace(subject))
            throw new ArgumentException("Email subject is required", nameof(subject));
        
        if (string.IsNullOrWhiteSpace(body))
            throw new ArgumentException("Email body is required", nameof(body));

        To = to;
        Subject = subject;
        Body = body;
        IsHtml = isHtml;
        CcRecipients = ccRecipients;
        BccRecipients = bccRecipients;
    }

    // Factory methods para diferentes tipos de emails
    public static EmailMessage CreateWelcomeEmail(string customerEmail, string customerName)
    {
        var subject = "Welcome to Firmness!";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: #2c3e50;'>Welcome to Firmness!</h2>
                <p Dear <strong>{customerName}</strong>,</p>
                <p>Your account has been successfully created.</p>
                <p>Now you can start enjoying our construction equipment rental services.</p>
                <br/>
                <p style='color: #7f8c8d;'>Greetings,<br/>The Firmnes Teams</p>
            </body>
            </html>";

        return new EmailMessage(customerEmail, subject, body, isHtml: true);
    }

    public static EmailMessage CreatePurchaseConfirmation(string customerEmail, string customerName, decimal totalAmount, string invoiceNumber)
    {
        var subject = $"Purchase Confirmation - Invoice #{invoiceNumber}";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: #27ae60;'>¡Thank you for your purchase!</h2>
                <p>Dear <strong>{customerName}</strong>,</p>
                <p>We have successfully received your payment.</p>
                <div style='background-color: #ecf0f1; padding: 15px; margin: 20px 0; border-radius: 5px;'>
                    <p><strong>Invoice Number:</strong> {invoiceNumber}</p>
                    <p><strong>Total:</strong> ${totalAmount:N2}</p>
                </div>
                <p>You can download your invoice from your customer panel.</p>
                <br/>
                <p style='color: #7f8c8d;'>Greetings,<br/>The Firmness Team</p>
            </body>
            </html>";

        return new EmailMessage(customerEmail, subject, body, isHtml: true);
    }
}
