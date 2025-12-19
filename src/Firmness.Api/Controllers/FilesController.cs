using Firmness.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmness.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<FilesController> _logger;

    // Allowed file extensions and max file size
    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public FilesController(
        IFileStorageService fileStorageService,
        ILogger<FilesController> logger)
    {
        _fileStorageService = fileStorageService;
        _logger = logger;
    }
    
    // Upload a product image
    [HttpPost("upload/product")]
    [ProducesResponseType(typeof(FileUploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadProductImage(IFormFile file)
    {
        return await UploadImage(file, "products");
    }
    
    // Upload a vehicle image
    [HttpPost("upload/vehicle")]
    [ProducesResponseType(typeof(FileUploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadVehicleImage(IFormFile file)
    {
        return await UploadImage(file, "vehicles");
    }


    // Upload a profile image
    [HttpPost("upload/profile")]
    [ProducesResponseType(typeof(FileUploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadProfileImage(IFormFile file)
    {
        return await UploadImage(file, "profiles");
    }
    
    // Delete a file by URL
    [HttpDelete]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteFile([FromQuery] string fileUrl)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return BadRequest(new { message = "File URL is required" });

            await _fileStorageService.DeleteFileAsync(fileUrl);
            
            _logger.LogInformation("File deleted: {FileUrl}", fileUrl);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FileUrl}", fileUrl);
            return StatusCode(500, new { message = "Error deleting file" });
        }
    }

    private async Task<IActionResult> UploadImage(IFormFile file, string folder)
    {
        try
        {
            // Validate file
            var validationError = ValidateImageFile(file);
            if (validationError != null)
                return BadRequest(new { message = validationError });

            // Upload to S3
            using var stream = file.OpenReadStream();
            var fileUrl = await _fileStorageService.UploadFileAsync(
                file.FileName,
                stream,
                file.ContentType,
                folder
            );

            _logger.LogInformation("Image uploaded successfully: {FileUrl}", fileUrl);

            return Ok(new FileUploadResponse
            {
                Url = fileUrl,
                FileName = file.FileName,
                Size = file.Length,
                ContentType = file.ContentType
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image");
            return StatusCode(500, new { message = "Error uploading image. Please try again." });
        }
    }

    private string? ValidateImageFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return "No file provided";

        if (file.Length > MaxFileSize)
            return $"File size exceeds maximum allowed size of {MaxFileSize / 1024 / 1024} MB";

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(extension))
            return $"Invalid file type. Allowed types: {string.Join(", ", AllowedImageExtensions)}";

        return null;
    }
}

public class FileUploadResponse
{
    public string Url { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long Size { get; set; }
    public string ContentType { get; set; } = string.Empty;
}
