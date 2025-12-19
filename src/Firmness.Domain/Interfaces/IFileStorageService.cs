namespace Firmness.Domain.Interfaces;

// Interface for file storage service (S3, Azure Blob, etc.)

public interface IFileStorageService
{
    /// <param name="fileName">Name of the file</param>
    /// <param name="fileStream">File content stream</param>
    /// <param name="contentType">MIME type of the file</param>
    /// <param name="folder">Optional folder/prefix</param>
    /// <returns>Public URL of the uploaded file</returns>
    Task<string> UploadFileAsync(string fileName, Stream fileStream, string contentType, string? folder = null);
    
    // Delete a file from storage
    /// <param name="fileUrl">URL or key of the file to delete</param>
    Task DeleteFileAsync(string fileUrl);
    
    // Get a pre-signed URL for temporary access
    /// <param name="fileKey">Key/path of the file</param>
    /// <param name="expirationMinutes">Expiration time in minutes</param>
    /// <returns>Pre-signed URL</returns>
    Task<string> GetPresignedUrlAsync(string fileKey, int expirationMinutes = 60);

  
    // Check if a file exists
    /// <param name="fileKey">Key/path of the file</param>
    Task<bool> FileExistsAsync(string fileKey);
}
