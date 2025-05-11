using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Models;
using UrlShortener.Utils;

namespace UrlShortener.Controllers;

public class ShortUrlController : Controller
{
    private readonly AppDbContext _db;

    public ShortUrlController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var urls = await _db.ShortUrls.Include(u => u.CreatedByUser).ToListAsync();
        return View(urls);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Add(string originalUrl)
    {
        if (string.IsNullOrWhiteSpace(originalUrl))
            return BadRequest(new { message = "URL cannot be empty." });

        if (!originalUrl.StartsWith("http://") && !originalUrl.StartsWith("https://"))
        {
            originalUrl = "https://" + originalUrl;
        }

        var existing = await _db.ShortUrls.FirstOrDefaultAsync(u => u.OriginalUrl == originalUrl);
        if (existing != null)
        {
            return BadRequest(new { message = "This URL already exists." });
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
        if (user == null)
            return Unauthorized();

        var shortUrl = new ShortUrl
        {
            OriginalUrl = originalUrl,
            CreatedDate = DateTime.UtcNow,
            CreatedByUserId = user.Id
        };

        _db.ShortUrls.Add(shortUrl);
        await _db.SaveChangesAsync();

        shortUrl.ShortCode = Base62Converter.Encode(shortUrl.Id);
        _db.ShortUrls.Update(shortUrl);
        await _db.SaveChangesAsync();

        return Json(new
        {
            shortUrl = new
            {
                shortUrl.Id,
                shortUrl.ShortCode,
                shortUrl.OriginalUrl,
                CreatedBy = user.Login
            }
        });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var url = await _db.ShortUrls
            .Include(u => u.CreatedByUser)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (url == null)
            return NotFound();

        if (User.IsInRole("Admin") || url.CreatedByUser.Login == User.Identity.Name)
        {
            _db.ShortUrls.Remove(url);
            await _db.SaveChangesAsync();
            return Ok();
        }

        return Forbid();
    }

    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        var url = await _db.ShortUrls.Include(u => u.CreatedByUser).FirstOrDefaultAsync(u => u.Id == id);

        if (url == null)
            return NotFound();

        return View(url);
    }
}
