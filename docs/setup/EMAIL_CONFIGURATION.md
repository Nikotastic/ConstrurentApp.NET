# 📧 Email Configuration Guide

Complete guide for configuring email functionality in Firmness using **Gmail SMTP** or other providers.

## 🎯 Overview

| Feature                   | Description                                | Trigger              |
| ------------------------- | ------------------------------------------ | -------------------- |
| **📧 Purchase Receipts**  | Send PDF receipts after successful payment | Payment Confirmation |
| **🔐 Account Activation** | Email with activation link for new users   | User Registration    |
| **📬 Notifications**      | General system notifications               | Various Events       |

---

## ⚠️ Important Notes

- **Email is OPTIONAL** - The API works without email configuration
- Without email: Users can register, but won't receive welcome emails
- **Never commit credentials** - Use `.env` or user secrets

---

## 🛠️ Gmail Setup (Recommended)

### Step 1: Enable 2-Step Verification

1. Go to [Google Account Security](https://myaccount.google.com/security)
2. Click **2-Step Verification** and enable it
3. Verify with your phone

### Step 2: Generate App Password

1. Go to [App Passwords](https://myaccount.google.com/apppasswords)
2. Select app: **Mail**
3. Select device: **Other (Custom name)** → Enter: `Firmness App`
4. Click **Generate**
5. **Copy the 16-character password** (you won't see it again)

---

## 🔧 Configuration Options

### Option 1: Environment Variables (.env) - Recommended

Add to your `.env` file:

```env
# Gmail SMTP Configuration
EmailSettings__SmtpServer=smtp.gmail.com
EmailSettings__SmtpPort=587
EmailSettings__SenderEmail=your-email@gmail.com
EmailSettings__SenderName=Firmness Team
EmailSettings__Username=your-email@gmail.com
EmailSettings__Password=your-app-password-here
EmailSettings__EnableSsl=true
EmailSettings__TimeoutSeconds=30
```

### Option 2: User Secrets (Development)

```bash
cd src/Firmness.Api
dotnet user-secrets set "EmailSettings:SmtpServer" "smtp.gmail.com"
dotnet user-secrets set "EmailSettings:SmtpPort" "587"
dotnet user-secrets set "EmailSettings:SenderEmail" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:SenderName" "Firmness Team"
dotnet user-secrets set "EmailSettings:Username" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:Password" "your-app-password"
dotnet user-secrets set "EmailSettings:EnableSsl" "true"
```

### Option 3: Docker Compose

```yaml
# docker-compose.yml
environment:
  - EmailSettings__SmtpServer=smtp.gmail.com
  - EmailSettings__SmtpPort=587
  - EmailSettings__SenderEmail=your-email@gmail.com
  - EmailSettings__Username=your-email@gmail.com
  - EmailSettings__Password=your-app-password
  - EmailSettings__EnableSsl=true
```

---

## 💻 Usage Examples

### Send Simple Email

```csharp
var emailMessage = new EmailMessage
{
    To = "customer@example.com",
    Subject = "Welcome to Firmness!",
    Body = "Thank you for registering with us.",
    IsHtml = false
};

await _emailService.SendEmailAsync(emailMessage);
```

### Send Receipt with PDF Attachment

```csharp
var emailMessage = EmailMessage.CreateReceiptEmail(
    customerEmail: "customer@example.com",
    customerName: "John Doe",
    totalAmount: 1500.00m,
    invoiceNumber: "INV-2024-001",
    pdfContent: pdfBytes
);

await _emailService.SendEmailAsync(emailMessage);
```

### Send Account Activation Email

```csharp
var activationLink = $"https://firmness.com/activate?token={token}";

var emailMessage = new EmailMessage
{
    To = user.Email,
    Subject = "Activate Your Firmness Account",
    Body = $@"
        <h2>Welcome to Firmness!</h2>
        <p>Click the link below to activate your account:</p>
        <a href='{activationLink}'>Activate Account</a>
        <p>This link expires in 24 hours.</p>
    ",
    IsHtml = true
};

await _emailService.SendEmailAsync(emailMessage);
```

---

## ✅ Verify Configuration

### Check Logs on Startup

**If configured:**

```
✅ Email service configured: smtp.gmail.com:587 (your-email@gmail.com)
```

**If NOT configured:**

```
⚠️  Email service is not properly configured. Emails will not be sent.
Missing: SmtpServer, SenderEmail, Username, Password
```

### Test Email Sending

```powershell
# Use the test script
.\docs\api\test-api.ps1
```

Or manually with curl:

```bash
curl -X POST http://localhost:5000/api/test/email \
  -H "Content-Type: application/json" \
  -d '{"to": "test@example.com", "subject": "Test", "body": "Test email"}'
```

---

## � Alternative Email Providers

### SendGrid (Recommended for Production)

```env
EmailSettings__SmtpServer=smtp.sendgrid.net
EmailSettings__SmtpPort=587
EmailSettings__Username=apikey
EmailSettings__Password=your-sendgrid-api-key
```

**Benefits:**

- Free: 100 emails/day
- Better deliverability than Gmail
- Professional service

### AWS SES

```env
EmailSettings__SmtpServer=email-smtp.us-east-1.amazonaws.com
EmailSettings__SmtpPort=587
EmailSettings__Username=your-aws-smtp-username
EmailSettings__Password=your-aws-smtp-password
```

**Benefits:**

- Very economical
- Highly scalable
- Integrated with AWS ecosystem

### Mailgun

```env
EmailSettings__SmtpServer=smtp.mailgun.org
EmailSettings__SmtpPort=587
EmailSettings__Username=postmaster@yourdomain.com
EmailSettings__Password=your-mailgun-password
```

**Benefits:**

- Simple API
- Good for transactional emails
- Detailed analytics

---

## 🔧 Troubleshooting

### "Authentication failed"

**Solution:**

- Verify you're using an **App Password**, not your regular Gmail password
- Ensure 2-Step Verification is enabled
- Check username matches sender email
- Regenerate App Password if needed

### "SMTP timeout"

**Solution:**

- Increase `TimeoutSeconds` in configuration
- Check firewall/antivirus isn't blocking port 587
- Verify internet connection

### "Emails going to spam"

**Solution:**

- Add SPF record to your domain DNS
- Use a verified sender email
- Avoid spam trigger words in subject/body
- Consider using SendGrid or AWS SES for production

### "App Password not working"

**Solution:**

- Regenerate a new App Password
- Ensure no spaces when copying the password
- Verify 2-Step Verification is still enabled

---

## � Security Best Practices

1. **Never commit credentials** - Always use `.env` or user secrets
2. **Use App Passwords** - Never use your main Gmail password
3. **Rotate passwords regularly** - Generate new App Passwords periodically
4. **Monitor usage** - Check Gmail's sent folder for suspicious activity
5. **Rate limiting** - Implement email sending limits to prevent abuse

---

## 🚀 Production Deployment

### Azure App Service

```bash
az webapp config appsettings set --name your-app --resource-group your-rg --settings \
  EmailSettings__SmtpServer="smtp.gmail.com" \
  EmailSettings__SenderEmail="noreply@yourdomain.com" \
  EmailSettings__Password="your-app-password"
```

### AWS Elastic Beanstalk

```yaml
# .ebextensions/environment.config
option_settings:
  - option_name: EmailSettings__SmtpServer
    value: smtp.gmail.com
  - option_name: EmailSettings__SenderEmail
    value: noreply@yourdomain.com
  - option_name: EmailSettings__Password
    value: your-app-password
```

---

## 📚 Related Documentation

- **[Email Receipts](../building-data/EMAIL_RECEIPTS.md)** - Sending receipts with PDF attachments
- **[Environment Setup](ENVIRONMENT.md)** - Complete environment configuration
- **[Gmail SMTP Settings](https://support.google.com/mail/answer/7126229)** - Official Gmail documentation
- **[Google App Passwords](https://support.google.com/accounts/answer/185833)** - How to generate App Passwords

---

## 💡 Recommendations

### For Development

- Use your personal Gmail with App Password
- Or create a test email: `firmness.test@gmail.com`

### For Production

- Use a professional service: SendGrid, AWS SES, or Mailgun
- Or use corporate email: `noreply@yourdomain.com`
- Never use personal Gmail in production
