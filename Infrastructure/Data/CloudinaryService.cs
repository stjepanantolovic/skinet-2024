
using System.Text.Json;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using Core.Enums;

namespace Infrastructure.Data;


public class CloudinaryService : IImageService
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

    public async Task<ImageStorageItem> UploadImageAsync(
    IFormFile file,
    string? dimension,
    ResizeStrategy strategy = ResizeStrategy.Pad,
    OutputFormatMode formatMode = OutputFormatMode.Auto,
    string? padHex = "#FFFFFFFF",     // default: white, opaque
    long largeInputThresholdBytes = 1_500_000, // ~1.5 MB -> prefer WebP in Auto mode
    int quality = 85
)
    {
        if (file is null || file.Length == 0)
            throw new ArgumentException("File is empty", nameof(file));

        (int? w, int? h) = ParseDimension(dimension);
        if (w is null || h is null) { w = 1024; h = 1024; }

        var baseName = Path.GetFileNameWithoutExtension(file.FileName);
        var safeName = Sanitize(baseName);
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var publicId = $"{_folder}/{safeName}_{timestamp}";

        await using var input = file.OpenReadStream();
        using var image = await Image.LoadAsync(input); // auto-detects format

        // Orientation + resize
        image.Mutate(ctx =>
        {
            ctx.AutoOrient();
            var opts = strategy == ResizeStrategy.Fit
                ? new ResizeOptions
                {
                    Size = new  SixLabors.ImageSharp.Size(w!.Value, h!.Value),
                    Mode = ResizeMode.Max,
                    Sampler = KnownResamplers.Bicubic,
                    PremultiplyAlpha = true
                }
                : new ResizeOptions
                {
                    Size = new SixLabors.ImageSharp.Size(w!.Value, h!.Value),
                    Mode = ResizeMode.Pad,
                    Position = AnchorPositionMode.Center,
                    PadColor = ParseHexColor(padHex) ?? Color.White,
                    Sampler = KnownResamplers.Bicubic,
                    PremultiplyAlpha = true
                };

            ctx.Resize(opts);
        });

        // Determine if the image has alpha
        bool hasAlpha = image.PixelType.AlphaRepresentation != PixelAlphaRepresentation.None;

        // Decide output format/extension
        string ext;
        Func<Image, Stream, Task> saveAsync;

        (ext, saveAsync) = DecideEncoder(
            image,
            file,
            hasAlpha,
            formatMode,
            largeInputThresholdBytes,
            quality);

        await using var outStream = new MemoryStream();
        await saveAsync(image, outStream);
        outStream.Position = 0;

        var uploadFileName = $"{safeName}_{timestamp}.{ext}";

        // Upload final pixels as-is
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(uploadFileName, outStream),
            PublicId = publicId,
            Overwrite = false,
            UseFilename = false,
            Folder = _folder
        };

        var result = await _client.UploadAsync(uploadParams);

        if (result.StatusCode != System.Net.HttpStatusCode.OK &&
            result.StatusCode != System.Net.HttpStatusCode.Created)
        {
            throw new Exception($"Cloudinary upload failed: {result.Error?.Message ?? result.StatusCode.ToString()}");
        }

        var bytes = result.Bytes > 0 ? result.Bytes : outStream.Length;
        var url = result.SecureUrl?.ToString() ?? result.Url?.ToString() ?? string.Empty;

        return new ImageStorageItem(result.PublicId, uploadFileName, url, bytes);
    }

    // --- helpers ---

    private static (int? w, int? h) ParseDimension(string? dimension)
    {
        if (string.IsNullOrWhiteSpace(dimension)) return (null, null);
        var s = dimension.Trim().ToLowerInvariant().Replace('×', 'x');
        var m = Regex.Match(s, @"^\s*(\d{1,5})\s*x\s*(\d{1,5})\s*$");
        if (!m.Success) return (null, null);
        if (!int.TryParse(m.Groups[1].Value, out var w)) return (null, null);
        if (!int.TryParse(m.Groups[2].Value, out var h)) return (null, null);
        if (w <= 0 || h <= 0 || w > 10000 || h > 10000) return (null, null);
        return (w, h);
    }

    private static Color? ParseHexColor(string? hex)
    {
        if (string.IsNullOrWhiteSpace(hex)) return null;
        var s = hex.Trim().TrimStart('#');

        if (s.Length == 3)
        {
            var r = Convert.ToByte(new string(s[0], 2), 16);
            var g = Convert.ToByte(new string(s[1], 2), 16);
            var b = Convert.ToByte(new string(s[2], 2), 16);
            return Color.FromRgb(r, g, b);
        }
        if (s.Length == 6 || s.Length == 8)
        {
            var r = Convert.ToByte(s.Substring(0, 2), 16);
            var g = Convert.ToByte(s.Substring(2, 2), 16);
            var b = Convert.ToByte(s.Substring(4, 2), 16);
            if (s.Length == 8)
            {
                var a = Convert.ToByte(s.Substring(6, 2), 16);
                return Color.FromRgba(r, g, b, a);
            }
            return Color.FromRgb(r, g, b);
        }
        return null;
    }

    private static (string ext, Func<Image, Stream, Task> saveAsync) DecideEncoder(
        Image image,
        IFormFile originalFile,
        bool hasAlpha,
        OutputFormatMode mode,
        long largeInputThresholdBytes,
        int quality)
    {
        // Normalize original extension
        var origExt = Path.GetExtension(originalFile.FileName).TrimStart('.').ToLowerInvariant();

        // Helpers
        (string, Func<Image, Stream, Task>) jpeg() => ("jpg", async (img, s) =>
        {
            var enc = new JpegEncoder { Quality = quality };
            await img.SaveAsJpegAsync(s, enc);
        }
        );

        (string, Func<Image, Stream, Task>) png() => ("png", async (img, s) =>
        {
            var enc = new PngEncoder { CompressionLevel = PngCompressionLevel.DefaultCompression };
            await img.SaveAsPngAsync(s, enc);
        }
        );

        (string, Func<Image, Stream, Task>) webpLossy() => ("webp", async (img, s) =>
        {
            var enc = new WebpEncoder { Quality = quality, FileFormat = WebpFileFormatType.Lossy };
            await img.SaveAsWebpAsync(s, enc);
        }
        );

        (string, Func<Image, Stream, Task>) webpLossless() => ("webp", async (img, s) =>
        {
            var enc = new WebpEncoder { FileFormat = WebpFileFormatType.Lossless };
            await img.SaveAsWebpAsync(s, enc);
        }
        );

        // Forced modes
        switch (mode)
        {
            case OutputFormatMode.ForceJpeg: return jpeg();
            case OutputFormatMode.ForcePng: return png();
            case OutputFormatMode.ForceWebp: return hasAlpha ? webpLossless() : webpLossy();
            case OutputFormatMode.KeepOriginal:
                {
                    // Keep original when sensible; if unknown -> fall back to Auto logic
                    return origExt switch
                    {
                        "jpg" or "jpeg" => jpeg(),
                        "png" => png(),
                        "webp" => hasAlpha ? webpLossless() : webpLossy(),
                        _ => AutoPick()
                    };
                }
            case OutputFormatMode.Auto:
            default:
                return AutoPick();
        }

        (string, Func<Image, Stream, Task>) AutoPick()
        {
            // If input is large -> prefer WebP for size
            if (originalFile.Length >= largeInputThresholdBytes)
                return hasAlpha ? webpLossless() : webpLossy();

            // If transparency is needed -> PNG or WebP(L) (we’ll pick WebP lossless for generally smaller files)
            if (hasAlpha)
                return webpLossless();

            // Default for photos (opaque) -> JPEG
            return jpeg();
        }
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

