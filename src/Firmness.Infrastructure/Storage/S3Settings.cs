namespace Firmness.Infrastructure.Storage;

// AWS S3 Configuration Settings
public class S3Settings
{
    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = "us-east-1";
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string? CloudFrontDomain { get; set; }
    public bool UseCloudFront { get; set; } = false;
    

    // Folder structure for organizing files
    public string ProductImagesFolder { get; set; } = "products";
    public string VehicleImagesFolder { get; set; } = "vehicles";
    public string ProfileImagesFolder { get; set; } = "profiles";
    public string DocumentsFolder { get; set; } = "documents";
}
