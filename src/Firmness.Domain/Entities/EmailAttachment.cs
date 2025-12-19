namespace Firmness.Domain.Entities;

public class EmailAttachment
{
    public string FileName { get; private set; }
    public byte[] Content { get; private set; }
    public string ContentType { get; private set; }

    public EmailAttachment(string fileName, byte[] content, string contentType = "application/pdf")
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is required", nameof(fileName));
        
        if (content == null || content.Length == 0)
            throw new ArgumentException("Content is required", nameof(content));

        FileName = fileName;
        Content = content;
        ContentType = contentType;
    }
}
