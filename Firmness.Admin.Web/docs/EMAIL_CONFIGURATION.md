# Email Service Configuration

## 📋 Description

This module implements the email sending service following the principles of **Hexagonal Architecture (Ports and Adapters)**. The design allows for easy switching between different SMTP providers without modifying the core business logic.

## 🏗️ Architecture

### Layers and Responsibilities

```
┌─────────────────────────────────────────────────────────────┐
│                   CAPA DE DOMINIO                           │
│  - IEmailService (Puerto)                                   │
│  - EmailMessage (Entidad)                                   │
└─────────────────────────────────────────────────────────────┘
                          ▲
                          │
┌─────────────────────────────────────────────────────────────┐
│                 CAPA DE APLICACIÓN                          │
│  - INotificationService (Puerto)                            │
│  - NotificationService (Servicio de Aplicación)             │
│  - CustomerService (Orquestador)                            │
└─────────────────────────────────────────────────────────────┘
                          ▲
                          │
┌─────────────────────────────────────────────────────────────┐
│              CAPA DE INFRAESTRUCTURA                        │
│  - GmailEmailService (Adaptador para Gmail)                 │
│  - EnterpriseEmailService (Adaptador Empresarial)           │
│  - EmailSettings (Configuración)                            │
└─────────────────────────────────────────────────────────────┘
```

### Implemented Principles

1. **Dependency Inversion**: Internal layers define interfaces (ports) that external layers implement (adapters).
2. **Liskov Substitution**: Both email services implement the same contract.
3. **Open/Closed**: We can add new providers without modifying existing code.
4. **Single Responsibility**: Each layer has a well-defined responsibility.

## 🚀 Gmail Setup

### Step 1: Enable Two-Step Verification

1. Go to your Google Account (https://myaccount.google.com/).
2. Navigate to **Security**.
3. Enable **Two-Step Verification**.

### Step 2: Generate an Application Password

1. In **Security**, search for **Application Passwords**.
2. Select **Mail** and **Other (custom name)**.
3. Name the application as "Firmness System"
4. Copy the generated password (16 characters)

### Step 3: Configure appsettings.Development.json

```json
{
  "EmailSettings": {
    "Provider": "Gmail",
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "tu-email@gmail.com",
    "SenderName": "Firmness System",
    "Username": "tu-email@gmail.com",
    "Password": "xxxx xxxx xxxx xxxx",  "EnableSsl": true,

"TimeoutSeconds": 30

}
}
```

## 🏢 Enterprise Configuration

To use an enterprise SMTP server, simply change the provider in `appsettings.Production.json`:

```json
{
  "EmailSettings": {
    "Provider": "Enterprise",
    "SmtpServer": "smtp.tuempresa.com",
    "SmtpPort": 587,
    "SenderEmail": "noreply@tuempresa.com",
    "SenderName": "Sistema Firmness",
    "Username": "usuario-smtp",
    "Password": "contraseña-segura",
    "EnableSsl": true,
    "TimeoutSeconds": 60
  }
}
```

## 📧 Supported Notification Types

### 1. Welcome Email
Sent automatically when a new customer registers.

``csharp
await _notificationService.SendWelcomeEmailAsync(customer);

``

### 2. Purchase Confirmation
Sent after processing a sale.

```csharp
await _notificationService.SendPurchaseConfirmationAsync(
    customer, 
    totalAmount, 
    invoiceNumber
);
```

### 3. Vehicle Rental Confirmation
Sent when a vehicle reservation is confirmed.

```csharp
await _notificationService.SendVehicleRentalConfirmationAsync(
    customer,
    vehicleName,
    startDate,
    endDate,
    totalAmount
);
```

### 4. Return Reminder
Sent to remind you to return a vehicle.

```csharp
await _notificationService.SendVehicleReturnReminderAsync(
    customer,
    vehicleName,
    returnDate
);
```

## 🔧 Usage in Code

### Example: Customer Registration with Welcome Email

```csharp
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepo;
    private readonly INotificationService _notificationService;

    public async Task AddAsync(Customer customer)
    {
       
        await _customerRepo.AddAsync(customer);
        
        try
        {
            await _notificationService.SendWelcomeEmailAsync(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando email a {Email}", customer.Email);
        }
    }
}
```

## 🧪 Testing

### Create a Test Mock
```csharp
public class MockEmailService : IEmailService
{
    public List<EmailMessage> SentEmails { get; } = new();
    
    public Task SendEmailAsync(EmailMessage message, CancellationToken ct = default)
    {
        SentEmails.Add(message);
        return Task.CompletedTask;
    }
    
    public Task SendBulkEmailAsync(IEnumerable<EmailMessage> messages, CancellationToken ct = default)
    {
        SentEmails.AddRange(messages);
        return Task.CompletedTask;
    }
}
```

## 🔐 Security

### Environment Variables (Recommended for Production)
```bash
# PowerShell
$env:EmailSettings__Password = "your-password-secure""

# Linux/Mac
export EmailSettings__Password="your-password-secure""
```

### User Secrets (Local Development)

```bash
dotnet user-secrets init
dotnet user-secrets set "EmailSettings:Password" "tu-password"
```

## 🐛 Troubleshooting

### Error: "The SMTP server requires a secure connection"
- Verify that `EnableSsl` is set to `true`
- Ensure you are using port 587 (TLS) or 465 (SSL)

### Error: "Authentication failed"
- Verify that you are using an **Application Password**, not your regular password
- Confirm that two-step verification is enabled

### Error: "Timeout"
- Increase `TimeoutSeconds` in the settings
- Check your network connection and firewall



