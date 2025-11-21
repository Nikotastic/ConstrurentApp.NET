using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;

namespace Firmness.Test.Mocks;

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