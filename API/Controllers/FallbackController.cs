using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class FallbackController : Controller
{
    public IActionResult Index()
    {
        var test = PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
            "wwwroot", "index.html"), "text/HTML");

        return test;
    }
}