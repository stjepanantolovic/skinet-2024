
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _client;
    private readonly string _folder;

    public CloudinaryService(IConfiguration config)
    {
        var cloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME")
                        ?? config["Cloudinary:CloudName"];
        var apiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY")
                        ?? config["Cloudinary:ApiKey"];
        var apiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")
                        ?? config["Cloudinary:ApiSecret"];
        _folder = config["Cloudinary:Folder"] ?? "images";

        if (string.IsNullOrWhiteSpace(cloudName) ||
            string.IsNullOrWhiteSpace(apiKey) ||
            string.IsNullOrWhiteSpace(apiSecret))
        {
            throw new ArgumentException("Cloudinary credentials are not configured.");
        }

        var account = new Account(cloudName, apiKey, apiSecret);
        _client = new Cloudinary(account);
    }

    public async Task<ImageStorageItem> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty", nameof(file));

        var baseName = Path.GetFileNameWithoutExtension(file.FileName);
        var safeName = Sanitize(baseName);
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var publicId = $"{_folder}/{safeName}_{timestamp}";

        using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            PublicId = publicId,
            Overwrite = false,
            UseFilename = false,
            Folder = _folder,
            Transformation = new Transformation().Width(630).Height(640).Crop("limit")
        };

        var result = await _client.UploadAsync(uploadParams);

        if (result.StatusCode != System.Net.HttpStatusCode.OK &&
            result.StatusCode != System.Net.HttpStatusCode.Created)
        {
            throw new Exception($"Cloudinary upload failed: {result.Error?.Message ?? result.StatusCode.ToString()}");
        }

        var bytes = result.Bytes > 0 ? result.Bytes : file.Length;
        var url = result.SecureUrl?.ToString() ?? result.Url?.ToString() ?? string.Empty;

        return new ImageStorageItem(result.PublicId, file.FileName, url, bytes);
    }

    public async Task<List<ImageStorageItem>> ListImagesAsync(int maxResults = 100)
    {
        var list = new List<ImageStorageItem>();

        var listParams = new ListResourcesParams
        {
            ResourceType = ResourceType.Image,
            Type = "upload",
            MaxResults = maxResults
        };

        var result = await _client.ListResourcesAsync(listParams);

        if (result.Resources == null || result.Resources.Count() == 0)
            return list;

        // Filter manually by folder
        foreach (var r in result.Resources)
        {
            if (!r.PublicId.StartsWith(_folder + "/"))
                continue;

            list.Add(new ImageStorageItem(
                r.PublicId,
                Path.GetFileName(r.PublicId),
                r.SecureUrl?.ToString() ?? r.Url?.ToString() ?? string.Empty,
                r.Bytes > 0 ? r.Bytes : 0
            ));
        }

        return list;
    }

    public async Task DeleteImageAsync(string publicId)
    {
        if (string.IsNullOrWhiteSpace(publicId))
            throw new ArgumentNullException(nameof(publicId));

        var destroyParams = new DeletionParams(publicId);
        var result = await _client.DestroyAsync(destroyParams);

        if (result.Result != "ok" && result.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception($"Cloudinary delete failed: {result.Result} / {result.Error?.Message}");
        }
    }

    private static string Sanitize(string name)
    {
        var cleaned = new string(name.Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_').ToArray());
        return string.IsNullOrEmpty(cleaned) ? Guid.NewGuid().ToString("n") : cleaned;
    }
}

