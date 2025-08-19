using Core.Enums;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly IImageService _imageService;

    public ImagesController(IImageService imageService)
    {
        _imageService = imageService;
    }

   [HttpPost]
public async Task<IActionResult> Upload(
    [FromForm] IFormFile file,
    [FromForm] string? dimension,
    [FromForm] string? strategy,     // "Fit" | "Pad"
    [FromForm] string? padHex,       // e.g. "#FFFFFFFF" or "#00000000"
    [FromForm] string? formatMode)
{
    if (file is null) return BadRequest("file is required.");

    var strat = Enum.TryParse<ResizeStrategy>(strategy, true, out var s) ? s : ResizeStrategy.Pad;
    var fmt   = Enum.TryParse<OutputFormatMode>(formatMode, true, out var f) ? f : OutputFormatMode.Auto;

    var result = await _imageService.UploadImageAsync(file, dimension, strat, fmt, padHex);
    return Ok(result);
}

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var images = await _imageService.ListImagesAsync();
        return Ok(images);
    }

    // Delete by publicId (e.g. "images/myname_20250817...")
    [HttpDelete("{publicId}")]
    public async Task<IActionResult> Delete(string publicId)
    {
        await _imageService.DeleteImageAsync(publicId);
        return Ok(new { message = "deleted", publicId });
    }
}
