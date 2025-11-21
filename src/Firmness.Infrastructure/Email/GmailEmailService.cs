using System.Net;
using System.Net.Mail;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Firmness.Infrastructure.Email;

public class GmailEmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<GmailEmailService> _logger;

    public GmailEmailService(
        IOptions<EmailSettings> settings,
        ILogger<GmailEmailService> logger)
    {
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        ValidateSettings();
    }

    public async Task SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured())
        {
            _logger.LogWarning(
                "⚠️  Email service not configured. Skipping email to {To} with subject: {Subject}",
                message.To,
                message.Subject);
            return;
        }
        
        try
        {
            _logger.LogInformation(
                "📧 Sending email to {To} with subject: {Subject}",
                message.To,
                message.Subject);

            using var smtpClient = CreateSmtpClient();
            using var mailMessage = CreateMailMessage(message);

            await smtpClient.SendMailAsync(mailMessage, cancellationToken);

            _logger.LogInformation(
                "✅ Email successfully sent to {To}",
                message.To);
        }
        catch (SmtpException ex)
        {
            _logger.LogError(
                ex,
                " SMTP error sending email to {To}: {Error}",
                message.To,
                ex.Message);
            throw new InvalidOperationException($"Error sending email: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                " Unexpected error sending email to {To}",
                message.To);
            throw;
        }
    }

    public async Task SendBulkEmailAsync(
        IEnumerable<EmailMessage> messages,
        CancellationToken cancellationToken = default)
    {
        var emailList = messages.ToList();
        _logger.LogInformation("📧 Sending {Count} emails in batch", emailList.Count);

        var tasks = emailList.Select(m => SendEmailAsync(m, cancellationToken));
        await Task.WhenAll(tasks);

        _logger.LogInformation("Batch of {Count} emails sent", emailList.Count);
    }

    private SmtpClient CreateSmtpClient()
    {
        return new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
        {
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            EnableSsl = _settings.EnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Timeout = _settings.TimeoutSeconds * 1000
        };
    }

    private MailMessage CreateMailMessage(EmailMessage message)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = message.IsHtml,
            Priority = MailPriority.Normal
        };

        mailMessage.To.Add(message.To);

        if (message.CcRecipients != null)
        {
            foreach (var cc in message.CcRecipients)
            {
                mailMessage.CC.Add(cc);
            }
        }

        if (message.BccRecipients != null)
        {
            foreach (var bcc in message.BccRecipients)
            {
                mailMessage.Bcc.Add(bcc);
            }
        }

        return mailMessage;
    }

    private void ValidateSettings()
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(_settings.SmtpServer))
            errors.Add("SmtpServer is not configured");

        if (string.IsNullOrWhiteSpace(_settings.SenderEmail))
            errors.Add("SenderEmail is not configured");

        if (string.IsNullOrWhiteSpace(_settings.Username))
            errors.Add("Username is not configured");

        if (string.IsNullOrWhiteSpace(_settings.Password))
            errors.Add("Password is not configured");

        if (errors.Any())
        {
            _logger.LogWarning(
                "⚠️  Email service is not properly configured. Emails will not be sent. Missing: {MissingSettings}",
                string.Join(", ", errors));
        }
        else
        {
            _logger.LogInformation(
                "✅ Email service configured: {SmtpServer}:{SmtpPort} ({SenderEmail})",
                _settings.SmtpServer,
                _settings.SmtpPort,
                _settings.SenderEmail);
        }
    }
    
    private bool IsConfigured()
    {
        return !string.IsNullOrWhiteSpace(_settings.SmtpServer) &&
               !string.IsNullOrWhiteSpace(_settings.SenderEmail) &&
               !string.IsNullOrWhiteSpace(_settings.Username) &&
               !string.IsNullOrWhiteSpace(_settings.Password);
    }
}