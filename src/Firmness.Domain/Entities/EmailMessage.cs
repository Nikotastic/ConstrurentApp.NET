namespace Firmness.Domain.Entities;

public class EmailMessage
{
     public string To { get; private set; }
    public string Subject { get; private set; }
    public string Body { get; private set; }
    public bool IsHtml { get; private set; }
    public List<string>? CcRecipients { get; private set; }
    public List<string>? BccRecipients { get; private set; }
    public List<EmailAttachment>? Attachments { get; private set; }

    public EmailMessage(
        string to,
        string subject,
        string body,
        bool isHtml = true,
        List<string>? ccRecipients = null,
        List<string>? bccRecipients = null,
        List<EmailAttachment>? attachments = null)
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
        Attachments = attachments;
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

    // Create purchase confirmation email
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
    // Create account activation email
    public static EmailMessage CreateAccountActivationEmail(string customerEmail, string customerName, string activationLink)
    {
        var subject = "Activate your Firmness account";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: #2c3e50;'>Welcome to Firmness!</h2>
                <p>Dear <strong>{customerName}</strong>,</p>
                <p>Your account has been created. To activate it and set your password, please click the link below:</p>
                <p style='text-align: center; margin: 30px 0;'>
                    <a href='{activationLink}' style='background-color: #3498db; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; font-weight: bold;'>Activate Account</a>
                </p>
                <p>If the button doesn't work, copy and paste this link into your browser:</p>
                <p>{activationLink}</p>
                <br/>
                <p style='color: #7f8c8d;'>Greetings,<br/>The Firmness Team</p>
            </body>
            </html>";

        return new EmailMessage(customerEmail, subject, body, isHtml: true);
    }

    // Create password reset email
    public static EmailMessage CreatePasswordResetEmail(string customerEmail, string customerName, string resetLink)
    {
        var subject = "Reset your password - Firmness";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: #e74c3c;'>Password Reset Request</h2>
                <p>Dear <strong>{customerName}</strong>,</p>
                <p>We received a request to reset your password. If you didn't make this request, you can ignore this email.</p>
                <p>To reset your password, click the link below:</p>
                <p style='text-align: center; margin: 30px 0;'>
                    <a href='{resetLink}' style='background-color: #e74c3c; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; font-weight: bold;'>Reset Password</a>
                </p>
                <p>If the button doesn't work, copy and paste this link into your browser:</p>
                <p>{resetLink}</p>
                <p><strong>This link will expire in 24 hours.</strong></p>
                <br/>
                <p style='color: #7f8c8d;'>Greetings,<br/>The Firmness Team</p>
            </body>
            </html>";

        return new EmailMessage(customerEmail, subject, body, isHtml: true);
    }
    
    // Create receipt email with PDF attachment
    public static EmailMessage CreateReceiptEmail(
        string customerEmail, 
        string customerName, 
        decimal totalAmount, 
        string invoiceNumber,
        byte[] pdfContent)
    {
        var subject = $"Your Receipt - Invoice #{invoiceNumber}";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: #27ae60;'>Thank you for your purchase!</h2>
                <p>Dear <strong>{customerName}</strong>,</p>
                <p>Please find attached your receipt for the recent purchase.</p>
                <div style='background-color: #ecf0f1; padding: 15px; margin: 20px 0; border-radius: 5px;'>
                    <p><strong>Invoice Number:</strong> {invoiceNumber}</p>
                    <p><strong>Total:</strong> ${totalAmount:N2}</p>
                </div>
                <p>The PDF receipt is attached to this email for your records.</p>
                <br/>
                <p style='color: #7f8c8d;'>Greetings,<br/>The Firmness Team</p>
            </body>
            </html>";

        var attachment = new EmailAttachment($"Receipt_{invoiceNumber}.pdf", pdfContent);
        
        return new EmailMessage(
            customerEmail, 
            subject, 
            body, 
            isHtml: true,
            attachments: new List<EmailAttachment> { attachment });
    }
}
