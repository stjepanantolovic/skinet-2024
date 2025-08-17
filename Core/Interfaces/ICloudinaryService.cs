using Core.Entities;
using Microsoft.AspNetCore.Http;
namespace Core.Interfaces;

public interface ICloudinaryService
{
    /// <summary> Uploads the provided IFormFile to Cloudinary and returns the uploaded image metadata (public id, url, size). </summary>
    Task<ImageStorageItem> UploadImageAsync(IFormFile file);

    /// <summary> Lists images in configured folder. </summary>
    Task<List<ImageStorageItem>> ListImagesAsync(int maxResults = 100);

    /// <summary> Deletes an image by Cloudinary public id (or full public id with folder). </summary>
    Task DeleteImageAsync(string publicId);
}


