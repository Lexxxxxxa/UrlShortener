using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Models;

namespace UrlShortener.Controllers;

[ApiController]
[Route("")]
public class RedirectController : Controller
{
    private readonly AppDbContext _db;

    public RedirectController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("{shortCode}")]
    public async Task<IActionResult> RedirectToOriginal(string shortCode)
    {
        if (string.IsNullOrWhiteSpace(shortCode))
            return BadRequest("Short code is required.");

        var url = await _db.ShortUrls.FirstOrDefaultAsync(u => u.ShortCode == shortCode);
        if (url == null)
            return NotFound("Short URL not found.");

        return Redirect(url.OriginalUrl);
    }
}
