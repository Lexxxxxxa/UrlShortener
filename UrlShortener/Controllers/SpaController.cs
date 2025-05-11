using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.Controllers;

public class SpaController : Controller
{
    [HttpGet("/shorturls")]
    public IActionResult Index()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "ClientApp", "dist", "client-app", "browser", "index.html");
        return PhysicalFile(filePath, "text/html");
    }
}
