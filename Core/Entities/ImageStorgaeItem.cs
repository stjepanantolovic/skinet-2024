using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ImageStorageItem
{
    /// <summary> Cloudinary public ID (unique identifier in Cloudinary, includes folder if used). </summary>
    public string PublicId { get; set; } = string.Empty;

    /// <summary> Original file name uploaded by the user. </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary> URL to access the image (HTTPS). </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary> File size in bytes. </summary>
    public long Bytes { get; set; }

    public ImageStorageItem() { }

    public ImageStorageItem(string publicId, string fileName, string url, long bytes)
    {
        PublicId = publicId;
        FileName = fileName;
        Url = url;
        Bytes = bytes;
    }
}
}