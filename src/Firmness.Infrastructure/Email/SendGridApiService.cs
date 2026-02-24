using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Firmness.Infrastructure.Email;

public class SendGridApiService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SendGridApiService> _logger;
    private readonly HttpClient _httpClient;

    public SendGridApiService(
        IOptions<EmailSettings> settings,
        ILogger<SendGridApiService> logger,
        HttpClient httpClient)
    {
        _settings = settings.Value;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_settings.Password))
        {
            _logger.LogWarning("⚠️ SendGrid API Key (Password) not set. Skipping email.");
            return;
        }

        try
        {
            var sendGridMessage = new
            {
                personalizations = new[]
                {
                    new { to = new[] { new { email = message.To } } }
                },
                from = new { email = _settings.SenderEmail, name = _settings.SenderName },
                subject = message.Subject,
                content = new[]
                {
                    new { type = message.IsHtml ? "text/html" : "text/plain", value = message.Body }
                },
                attachments = message.Attachments?.Select(a => new
                {
                    content = Convert.ToBase64String(a.Content),
                    filename = a.FileName,
                    type = a.ContentType,
                    disposition = "attachment"
                }).ToArray()
            };

            var json = JsonSerializer.Serialize(sendGridMessage);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.sendgrid.com/v3/mail/send");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.Password);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("✅ Email sent successfully via SendGrid API to {To}", message.To);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("❌ Failed to send email via SendGrid API. Status: {Status}, Error: {Error}", response.StatusCode, error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Unexpected error sending email via SendGrid API");
        }
    }

    public async Task SendBulkEmailAsync(IEnumerable<EmailMessage> messages, CancellationToken cancellationToken = default)
    {
        foreach (var message in messages)
        {
            await SendEmailAsync(message, cancellationToken);
        }
    }
}
