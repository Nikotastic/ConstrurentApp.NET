# ☁️ AWS S3 Storage Integration

Manage file uploads and cloud storage using **AWS S3** with CloudFront CDN support for fast content delivery.

## 🚀 Features Overview

| Feature                 | Description                              | Use Case                   |
| ----------------------- | ---------------------------------------- | -------------------------- |
| **📸 Image Upload**     | Store product and vehicle images in S3   | Product/Vehicle Management |
| **📄 Document Storage** | Upload contracts, invoices, and receipts | Legal Documents            |
| **🌐 CloudFront CDN**   | Fast global content delivery             | Performance Optimization   |
| **🔒 Secure Access**    | Pre-signed URLs for temporary access     | Private File Sharing       |

---

## 📋 Prerequisites

- AWS Account with S3 access
- IAM User with S3 permissions
- S3 Bucket created
- (Optional) CloudFront distribution

---

## 🛠️ Configuration

### 1. AWS Setup

#### Create S3 Bucket

1. Go to [AWS S3 Console](https://console.aws.amazon.com/s3/)
2. Click **Create bucket**
3. Bucket name: `firmness-storage` (must be globally unique)
4. Region: `us-east-1` (or your preferred region)
5. **Block all public access**: Uncheck (we'll use IAM policies)
6. Click **Create bucket**

#### Create IAM User

1. Go to [AWS IAM Console](https://console.aws.amazon.com/iam/)
2. Create new user: `firmness-s3-user`
3. Attach policy: `AmazonS3FullAccess` (or create custom policy)
4. Generate **Access Key** and **Secret Key**

### 2. Environment Configuration

Add to your `.env` file:

```env
# AWS S3 Configuration
S3Settings__BucketName=firmness-storage
S3Settings__Region=us-east-1
S3Settings__AccessKey=AKIAIOSFODNN7EXAMPLE
S3Settings__SecretKey=wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY

# Optional: CloudFront CDN
S3Settings__UseCloudFront=false
S3Settings__CloudFrontDomain=d111111abcdef8.cloudfront.net
```

---

## 💻 Usage Examples

### Upload Image

```csharp
// In your controller or service
[HttpPost("vehicles/{id}/image")]
public async Task<IActionResult> UploadVehicleImage(Guid id, IFormFile file)
{
    // Upload to S3
    var imageUrl = await _fileStorageService.UploadFileAsync(
        file.OpenReadStream(),
        file.FileName,
        "vehicles"
    );

    // Update vehicle record
    await _vehicleService.UpdateImageUrlAsync(id, imageUrl);

    return Ok(new { imageUrl });
}
```

### Delete File

```csharp
// Delete old image when updating
if (!string.IsNullOrEmpty(vehicle.ImageUrl))
{
    await _fileStorageService.DeleteFileAsync(vehicle.ImageUrl);
}
```

### Generate Pre-signed URL

```csharp
// For temporary access to private files
var presignedUrl = await _fileStorageService.GetPresignedUrlAsync(
    fileKey: "contracts/contract-123.pdf",
    expirationMinutes: 60
);
```

---

## 🌐 CloudFront Integration (Optional)

CloudFront provides faster content delivery through a global CDN.

### Setup

1. Go to [CloudFront Console](https://console.aws.amazon.com/cloudfront/)
2. Create distribution
3. Set origin to your S3 bucket
4. Copy the CloudFront domain (e.g., `d111111abcdef8.cloudfront.net`)

### Enable in Configuration

```env
S3Settings__UseCloudFront=true
S3Settings__CloudFrontDomain=d111111abcdef8.cloudfront.net
```

When enabled, all file URLs will use CloudFront instead of direct S3 URLs.

---

## 🔧 Troubleshooting

### "Access Denied" Error

**Solution:**

- Verify IAM user has S3 permissions
- Check bucket policy allows your IAM user
- Ensure Access Key and Secret Key are correct

### "Bucket not found"

**Solution:**

- Verify bucket name in `.env` matches AWS
- Check region is correct
- Ensure bucket exists in your AWS account

### Images not loading

**Solution:**

- Check CORS configuration on S3 bucket
- Verify CloudFront distribution is deployed (if using)
- Check browser console for CORS errors

---

## 📚 Related Documentation

- **[Environment Setup](../setup/ENVIRONMENT.md)** - Complete environment configuration
- **[Email Configuration](../setup/EMAIL_CONFIGURATION.md)** - SMTP and email setup

---

## 🔗 External Resources

- [AWS S3 Documentation](https://docs.aws.amazon.com/s3/)
- [CloudFront Documentation](https://docs.aws.amazon.com/cloudfront/)
- [IAM Best Practices](https://docs.aws.amazon.com/IAM/latest/UserGuide/best-practices.html)
