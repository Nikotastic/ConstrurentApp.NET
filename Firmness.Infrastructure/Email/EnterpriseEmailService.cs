using System.Net;
using System.Net.Mail;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Firmness.Infrastructure.Email;

// Adapter for enterprise SMTP servers
public class EnterpriseEmailService : IEmailService
{
     private readonly EmailSettings _settings;
    private readonly ILogger<EnterpriseEmailService> _logger;

    public EnterpriseEmailService(
        IOptions<EmailSettings> settings,
        ILogger<EnterpriseEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "[Enterprise SMTP] 📧 Sending email to {To}",
                message.To);

            using var smtpClient = CreateSmtpClient();
            using var mailMessage = CreateMailMessage(message);

            await smtpClient.SendMailAsync(mailMessage, cancellationToken);

            _logger.LogInformation(
                "[Enterprise SMTP] Email sent to {To}",
                message.To);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[Enterprise SMTP] Error sending email to {To}",
                message.To);
            throw;
        }
    }

    public async Task SendBulkEmailAsync(
        IEnumerable<EmailMessage> messages,
        CancellationToken cancellationToken = default)
    {
        var tasks = messages.Select(m => SendEmailAsync(m, cancellationToken));
        await Task.WhenAll(tasks);
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
            IsBodyHtml = message.IsHtml
        };

        mailMessage.To.Add(message.To);

        if (message.CcRecipients != null)
            foreach (var cc in message.CcRecipients) mailMessage.CC.Add(cc);

        if (message.BccRecipients != null)
            foreach (var bcc in message.BccRecipients) mailMessage.Bcc.Add(bcc);

        return mailMessage;
    }
}