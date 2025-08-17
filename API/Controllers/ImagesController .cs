using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly ICloudinaryService _cloudinary;

    public ImagesController(ICloudinaryService cloudinary)
    {
        _cloudinary = cloudinary;
    }

    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        if (file == null) return BadRequest("No file uploaded.");
        var item = await _cloudinary.UploadImageAsync(file);
        return Ok(item);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var images = await _cloudinary.ListImagesAsync();
        return Ok(images);
    }

    // Delete by publicId (e.g. "images/myname_20250817...")
    [HttpDelete("{publicId}")]
    public async Task<IActionResult> Delete(string publicId)
    {
        await _cloudinary.DeleteImageAsync(publicId);
        return Ok(new { message = "deleted", publicId });
    }
}
