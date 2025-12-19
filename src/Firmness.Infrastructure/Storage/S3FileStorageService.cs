using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Firmness.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Firmness.Infrastructure.Storage;

// AWS S3 File Storage Service Implementation
public class S3FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3Settings _settings;
    private readonly ILogger<S3FileStorageService> _logger;

    public S3FileStorageService(
        IAmazonS3 s3Client,
        IOptions<S3Settings> settings,
        ILogger<S3FileStorageService> logger)
    {
        _s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        ValidateSettings();
    }

    public async Task<string> UploadFileAsync(
        string fileName, 
        Stream fileStream, 
        string contentType, 
        string? folder = null)
    {
        try
        {
            // Generate unique file name to avoid collisions
            var fileExtension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var key = string.IsNullOrEmpty(folder) 
                ? uniqueFileName 
                : $"{folder}/{uniqueFileName}";

            _logger.LogInformation("Uploading file to S3: {Key}", key);

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = fileStream,
                Key = key,
                BucketName = _settings.BucketName,
                ContentType = contentType
                // CannedACL removed as modern S3 buckets disable ACLs by default
            };

            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(uploadRequest);

            var fileUrl = GetPublicUrl(key);
            _logger.LogInformation("File uploaded successfully: {Url}", fileUrl);

            return fileUrl;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "S3 error uploading file: {FileName}", fileName);
            throw new InvalidOperationException($"Failed to upload file to S3: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", fileName);
            throw;
        }
    }

    public async Task DeleteFileAsync(string fileUrl)
    {
        try
        {
            var key = ExtractKeyFromUrl(fileUrl);
            
            _logger.LogInformation("Deleting file from S3: {Key}", key);

            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);
            
            _logger.LogInformation("File deleted successfully: {Key}", key);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "S3 error deleting file: {FileUrl}", fileUrl);
            throw new InvalidOperationException($"Failed to delete file from S3: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FileUrl}", fileUrl);
            throw;
        }
    }

    public async Task<string> GetPresignedUrlAsync(string fileKey, int expirationMinutes = 60)
    {
        try
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _settings.BucketName,
                Key = fileKey,
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes)
            };

            var url = await _s3Client.GetPreSignedURLAsync(request);
            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating presigned URL for: {FileKey}", fileKey);
            throw;
        }
    }

    public async Task<bool> FileExistsAsync(string fileKey)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _settings.BucketName,
                Key = fileKey
            };

            await _s3Client.GetObjectMetadataAsync(request);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking file existence: {FileKey}", fileKey);
            throw;
        }
    }

    private string GetPublicUrl(string key)
    {
        if (_settings.UseCloudFront && !string.IsNullOrEmpty(_settings.CloudFrontDomain))
        {
            return $"https://{_settings.CloudFrontDomain}/{key}";
        }

        return $"https://{_settings.BucketName}.s3.{_settings.Region}.amazonaws.com/{key}";
    }

    private string ExtractKeyFromUrl(string fileUrl)
    {
        // Extract key from S3 URL or CloudFront URL
        var uri = new Uri(fileUrl);
        
        if (_settings.UseCloudFront && fileUrl.Contains(_settings.CloudFrontDomain!))
        {
            return uri.AbsolutePath.TrimStart('/');
        }

        // For S3 URL: https://bucket.s3.region.amazonaws.com/key
        return uri.AbsolutePath.TrimStart('/');
    }

    private void ValidateSettings()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(_settings.BucketName))
            errors.Add("S3 BucketName is not configured");

        if (string.IsNullOrWhiteSpace(_settings.AccessKey))
            errors.Add("S3 AccessKey is not configured");

        if (string.IsNullOrWhiteSpace(_settings.SecretKey))
            errors.Add("S3 SecretKey is not configured");

        if (errors.Any())
        {
            _logger.LogWarning("S3 service is not properly configured. Missing: {MissingSettings}", 
                string.Join(", ", errors));
        }
        else
        {
            _logger.LogInformation("S3 service configured: Bucket={BucketName}, Region={Region}", 
                _settings.BucketName, _settings.Region);
        }
    }
}
