# 📧 Email Configuration - Firmness API

## ⚠️ Important Security Note

**DO NOT put passwords in `appsettings.json` files that are uploaded to Git.**

---

## 🎯 How Does Email Work?

### Without Configuration (Default) ✅
- The API **WORKS** without configuring email
- Users can register normally
- You'll only see a **warning** in the logs: *"Email service not configured"*
- Welcome emails are NOT sent, but registration works

### With Configuration (Optional) 📧
- Users receive welcome emails upon registration
- You can send email notifications

---

## 🔧 Option 1: Environment Variables (Recommended)

### For Development (Local):

**Windows PowerShell:**
```powershell
$env:EmailSettings__SmtpServer = "smtp.gmail.com"
$env:EmailSettings__SenderEmail = "your-email@gmail.com"
$env:EmailSettings__Username = "your-email@gmail.com"
$env:EmailSettings__Password = "your-app-password-here"
```

**Windows CMD:**
```cmd
set EmailSettings__SmtpServer=smtp.gmail.com
set EmailSettings__SenderEmail=your-email@gmail.com
set EmailSettings__Username=your-email@gmail.com
set EmailSettings__Password=your-app-password-here
```

**Then run the API:**
```powershell
cd C:\Users\NikoC\RiderProjects\ConstrurentApp.NET\src\Firmness.Api
dotnet run
```

---

## 🔧 Option 2: User Secrets (Recommended for Dev)

### 1. Initialize User Secrets:
```powershell
cd C:\Users\NikoC\RiderProjects\ConstrurentApp.NET\src\Firmness.Api
dotnet user-secrets init
```

### 2. Configure Email:
```powershell
dotnet user-secrets set "EmailSettings:SmtpServer" "smtp.gmail.com"
dotnet user-secrets set "EmailSettings:SmtpPort" "587"
dotnet user-secrets set "EmailSettings:SenderEmail" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:Username" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:Password" "your-app-password-here"
```

### 3. View configured secrets:
```powershell
dotnet user-secrets list
```

### 4. Clear secrets (if needed):
```powershell
dotnet user-secrets clear
```

---

## 🔧 Option 3: Local File (DO NOT upload to Git)

### 1. Create `appsettings.Local.json` file:

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderName": "Firmness System",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password-here",
    "EnableSsl": true,
    "TimeoutSeconds": 30
  }
}
```

### 2. Add to `.gitignore`:

```
appsettings.Local.json
```

### 3. Modify `Program.cs` to load this file:

```csharp
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true);
```

---

## 📧 How to Get a Gmail App Password

### Gmail requires "App Passwords" instead of your normal password:

1. **Go to your Google account:**
   - https://myaccount.google.com/

2. **Enable 2-Step Verification:**
   - Security → 2-Step Verification → Enable

3. **Create App Password:**
   - Security → App passwords
   - Select app: "Mail"
   - Select device: "Windows Computer"
   - Copy the generated password (16 characters)

4. **Use that password in the configuration**

---

## 🎯 Which Email to Use

### For Development (Testing):
- **Option A:** Your personal email (with App Password)
- **Option B:** Create a test email: `firmness.test@gmail.com`

### For Production:
- **Option A:** Corporate email: `noreply@yourdomain.com`
- **Option B:** Professional service: SendGrid, AWS SES, Mailgun

---

## ✅ Verify Configuration

### 1. Check logs when starting the API:

**If NOT configured:**
```
⚠️  Email service is not properly configured. Emails will not be sent.
Missing: SmtpServer, SenderEmail, Username, Password
```

**If configured:**
```
✅ Email service configured: smtp.gmail.com:587 (your-email@gmail.com)
```

### 2. Test user registration:

If email is configured:
```
📧 Sending email to user@example.com with subject: Welcome to Firmness
✅ Email successfully sent to user@example.com
```

If NOT configured:
```
⚠️  Email service not configured. Skipping email to user@example.com
```

---

## 🚀 For Production (Azure/AWS)

### Azure App Service:

```powershell
az webapp config appsettings set --name your-app --resource-group your-rg --settings \
  EmailSettings__SmtpServer="smtp.gmail.com" \
  EmailSettings__SenderEmail="noreply@yourdomain.com" \
  EmailSettings__Username="your-email@gmail.com" \
  EmailSettings__Password="your-app-password"
```

### AWS Elastic Beanstalk:

```yaml
# .ebextensions/environment.config
option_settings:
  - option_name: EmailSettings__SmtpServer
    value: smtp.gmail.com
  - option_name: EmailSettings__SenderEmail
    value: noreply@yourdomain.com
  - option_name: EmailSettings__Username
    value: your-email@gmail.com
  - option_name: EmailSettings__Password
    value: your-app-password
```

### Docker:

```dockerfile
ENV EmailSettings__SmtpServer="smtp.gmail.com"
ENV EmailSettings__SenderEmail="noreply@yourdomain.com"
ENV EmailSettings__Username="your-email@gmail.com"
ENV EmailSettings__Password="your-app-password"
```

Or using `docker-compose.yml`:

```yaml
environment:
  - EmailSettings__SmtpServer=smtp.gmail.com
  - EmailSettings__SenderEmail=noreply@yourdomain.com
  - EmailSettings__Username=your-email@gmail.com
  - EmailSettings__Password=your-app-password
```

---

## 📊 Alternative Email Providers

### SendGrid (Recommended for production)
- **Free:** 100 emails/day
- **More reliable** than Gmail
- **Better deliverability**

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.sendgrid.net",
    "SmtpPort": 587,
    "Username": "apikey",
    "Password": "your-sendgrid-api-key"
  }
}
```

### AWS SES
- **Very economical**
- **Scalable**

```json
{
  "EmailSettings": {
    "SmtpServer": "email-smtp.us-east-1.amazonaws.com",
    "SmtpPort": 587,
    "Username": "your-aws-smtp-username",
    "Password": "your-aws-smtp-password"
  }
}
```

### Mailgun
- **Simple API**
- **Good for transactional emails**

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.mailgun.org",
    "SmtpPort": 587,
    "Username": "postmaster@yourdomain.com",
    "Password": "your-mailgun-password"
  }
}
```

---

## 🐛 Troubleshooting

### Error: "Authentication failed"

**Cause:** Invalid App Password

**Solution:**
1. Generate a new App Password in Google
2. Make sure to copy all 16 characters without spaces
3. Update configuration

### Error: "SMTP server requires secure connection"

**Cause:** SSL/TLS not configured correctly

**Solution:**
```json
{
  "EmailSettings": {
    "EnableSsl": true,
    "SmtpPort": 587  // Use 587 with TLS
  }
}
```

### Emails not being sent (no errors)

**Solution:**
1. Check spam folder
2. Verify sender email is correct
3. Check Gmail "Less secure apps" settings
4. Use App Password instead of regular password

---

## 📚 References

- [Gmail App Passwords](https://support.google.com/accounts/answer/185833)
- [SendGrid Documentation](https://docs.sendgrid.com/)
- [AWS SES Documentation](https://docs.aws.amazon.com/ses/)

