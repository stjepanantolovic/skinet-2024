using Core.Entities;
using Core.Enums;
using Microsoft.AspNetCore.Http;
namespace Core.Interfaces;

public interface IImageService
{
    /// <summary> Uploads the provided IFormFile to Cloudinary and returns the uploaded image metadata (public id, url, size). </summary>
    Task<ImageStorageItem> UploadImageAsync(IFormFile file,
    string? dimension,
    ResizeStrategy strategy = ResizeStrategy.Pad,
    OutputFormatMode formatMode = OutputFormatMode.Auto,
    string? padHex = "#FFFFFFFF",     // default: white, opaque
    long largeInputThresholdBytes = 1_500_000, // ~1.5 MB -> prefer WebP in Auto mode
    int quality = 85);

    /// <summary> Lists images in configured folder. </summary>
    Task<List<ImageStorageItem>> ListImagesAsync(int maxResults = 100);

    /// <summary> Deletes an image by Cloudinary public id (or full public id with folder). </summary>
    Task DeleteImageAsync(string publicId);
}


