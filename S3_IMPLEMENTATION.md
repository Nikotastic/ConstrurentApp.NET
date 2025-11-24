# 📸 AWS S3 Image Storage - Implementation Guide

## 🎯 Overview

This implementation provides a complete AWS S3 integration for storing product images, vehicle images, profile pictures, and documents.

## 🏗️ Architecture

```
Frontend (Angular)
    ↓ Upload Image
API Controller (FilesController)
    ↓ Validate & Process
S3 File Storage Service
    ↓ Upload to AWS
AWS S3 Bucket
    ↓ Return Public URL
Database (Store URL)
```

## 📦 Components Created

### Backend

1. **`IFileStorageService`** - Interface for file storage operations
2. **`S3Settings`** - Configuration class for AWS credentials
3. **`S3FileStorageService`** - Implementation of S3 operations
4. **`FilesController`** - API endpoints for file upload/delete

### API Endpoints

```
POST /api/files/upload/product    - Upload product image
POST /api/files/upload/vehicle    - Upload vehicle image
POST /api/files/upload/profile    - Upload profile image
DELETE /api/files?fileUrl={url}    - Delete file (Admin only)
```

## ⚙️ Configuration

### 1. AWS Setup

#### Create S3 Bucket

```bash
# Using AWS CLI
aws s3 mb s3://your-bucket-name --region us-east-1

# Set bucket policy for public read
aws s3api put-bucket-policy --bucket your-bucket-name --policy file://bucket-policy.json
```

#### Bucket Policy (bucket-policy.json)

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "PublicReadGetObject",
      "Effect": "Allow",
      "Principal": "*",
      "Action": "s3:GetObject",
      "Resource": "arn:aws:s3:::your-bucket-name/*"
    }
  ]
}
```

#### Create IAM User

1. Go to AWS IAM Console
2. Create new user: `firmness-s3-user`
3. Attach policy: `AmazonS3FullAccess` (or custom policy)
4. Generate Access Key and Secret Key

### 2. Environment Variables

Add to your `.env` file:

```env
# AWS S3 Configuration
S3Settings__BucketName=your-bucket-name
S3Settings__Region=us-east-1
S3Settings__AccessKey=AKIAIOSFODNN7EXAMPLE
S3Settings__SecretKey=wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY

# Optional: CloudFront CDN
S3Settings__UseCloudFront=false
S3Settings__CloudFrontDomain=d111111abcdef8.cloudfront.net
```

### 3. Folder Structure

Files are organized in folders:

- `products/` - Product images
- `vehicles/` - Vehicle images
- `profiles/` - Profile pictures
- `documents/` - Documents

## 🚀 Usage

### Frontend (Angular)

#### Upload Product Image

```typescript
import { HttpClient } from "@angular/common/http";

export class ProductFormComponent {
  constructor(private http: HttpClient) {}

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    const file = input.files[0];
    const formData = new FormData();
    formData.append("file", file);

    this.http
      .post<FileUploadResponse>("/api/files/upload/product", formData)
      .subscribe({
        next: (response) => {
          console.log("Image uploaded:", response.url);
          this.product.imageUrl = response.url;
        },
        error: (error) => {
          console.error("Upload failed:", error);
        },
      });
  }
}
```

#### HTML Template

```html
<div class="form-group">
  <label>Product Image</label>
  <input type="file" accept="image/*" (change)="onFileSelected($event)" />

  @if (product.imageUrl) {
  <img [src]="product.imageUrl" alt="Product" class="preview" />
  }
</div>
```

### Backend (C#)

#### Use in Product Service

```csharp
public class ProductService
{
    private readonly IFileStorageService _fileStorage;

    public async Task<Product> CreateProductAsync(CreateProductDto dto, IFormFile? image)
    {
        var product = new Product { /* ... */ };

        if (image != null)
        {
            using var stream = image.OpenReadStream();
            var imageUrl = await _fileStorage.UploadFileAsync(
                image.FileName,
                stream,
                image.ContentType,
                "products"
            );

            product.ImageUrl = imageUrl;
        }

        await _repository.AddAsync(product);
        return product;
    }

    public async Task DeleteProductAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);

        // Delete image from S3
        if (!string.IsNullOrEmpty(product.ImageUrl))
        {
            await _fileStorage.DeleteFileAsync(product.ImageUrl);
        }

        await _repository.DeleteAsync(id);
    }
}
```

## 📝 File Validation

The API validates:

- ✅ File size (max 5 MB)
- ✅ File type (jpg, jpeg, png, gif, webp)
- ✅ File presence

## 🔒 Security

### Authentication

- All upload endpoints require authentication
- Delete endpoint requires Admin role

### File Naming

- Files are renamed with GUID to prevent collisions
- Original extension is preserved

### Public Access

- Files are publicly readable (for product images)
- For private files, use presigned URLs

## 🌐 CloudFront CDN (Optional)

For better performance, use CloudFront:

1. Create CloudFront distribution
2. Set origin to your S3 bucket
3. Update `.env`:

```env
S3Settings__UseCloudFront=true
S3Settings__CloudFrontDomain=d111111abcdef8.cloudfront.net
```

## 🧪 Testing

### Using Swagger

1. Go to `http://localhost:5000/swagger`
2. Authorize with JWT token
3. Try `/api/files/upload/product`
4. Select file and upload
5. Copy returned URL

### Using cURL

```bash
# Upload image
curl -X POST "http://localhost:5000/api/files/upload/product" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -F "file=@/path/to/image.jpg"

# Delete image
curl -X DELETE "http://localhost:5000/api/files?fileUrl=https://bucket.s3.amazonaws.com/products/abc123.jpg" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## 📊 Response Format

### Upload Success

```json
{
  "url": "https://your-bucket.s3.us-east-1.amazonaws.com/products/550e8400-e29b-41d4-a716-446655440000.jpg",
  "fileName": "product-image.jpg",
  "size": 245678,
  "contentType": "image/jpeg"
}
```

### Upload Error

```json
{
  "message": "File size exceeds maximum allowed size of 5 MB"
}
```

## 🔧 Troubleshooting

### "S3 service is not properly configured"

- Check that all S3 settings are in `.env`
- Verify AWS credentials are correct
- Check bucket name and region

### "Access Denied" when uploading

- Verify IAM user has S3 permissions
- Check bucket policy allows uploads

### Images not loading

- Verify bucket policy allows public read
- Check CORS configuration on bucket
- Ensure CloudFront domain is correct (if using)

## 💡 Best Practices

1. **Use CloudFront** for production (faster delivery)
2. **Compress images** before upload (frontend)
3. **Set expiration** on old/unused images
4. **Monitor costs** in AWS Console
5. **Backup bucket** with versioning enabled

## 🔜 Future Enhancements

- [ ] Image resizing/thumbnails (using Lambda)
- [ ] Multiple image upload
- [ ] Image cropping in frontend
- [ ] Video upload support
- [ ] Progress bar for uploads
- [ ] Drag & drop interface

## 📚 Resources

- [AWS S3 Documentation](https://docs.aws.amazon.com/s3/)
- [AWS SDK for .NET](https://docs.aws.amazon.com/sdk-for-net/)
- [CloudFront Documentation](https://docs.aws.amazon.com/cloudfront/)
