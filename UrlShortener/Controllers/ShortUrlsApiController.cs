using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Models;
using UrlShortener.Utils;

namespace UrlShortener.Controllers;

[Route("api/shorturls")]
[ApiController]
public class ShortUrlsApiController : ControllerBase
{
    private readonly AppDbContext _db;

    public ShortUrlsApiController(AppDbContext db)
    {
        _db = db;
    }

    public class AddUrlDto
    {
        public string Url { get; set; }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var urls = await _db.ShortUrls
            .Include(u => u.CreatedByUser)
            .Select(u => new {
                u.Id,
                u.OriginalUrl,
                u.ShortCode,
                u.CreatedDate,
                CreatedBy = u.CreatedByUser.Login
            })
            .ToListAsync();

        return Ok(urls);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddUrlDto dto)
    {
        var originalUrl = dto.Url;

        if (string.IsNullOrWhiteSpace(originalUrl))
            return BadRequest("URL cannot be empty.");

        var existing = await _db.ShortUrls.FirstOrDefaultAsync(u => u.OriginalUrl == originalUrl);
        if (existing != null)
            return BadRequest("This URL already exists.");

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
        await _db.SaveChangesAsync();

        return Ok(new
        {
            shortUrl.Id,
            shortUrl.OriginalUrl,
            shortUrl.ShortCode,
            shortUrl.CreatedDate,
            CreatedBy = user.Login
        });
    }


    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var url = await _db.ShortUrls.Include(x => x.CreatedByUser).FirstOrDefaultAsync(x => x.Id == id);
        if (url == null)
            return NotFound();

        var currentUser = User.Identity.Name;
        var isAdmin = User.IsInRole("Admin");

        if (isAdmin || url.CreatedByUser.Login == currentUser)
        {
            _db.ShortUrls.Remove(url);
            await _db.SaveChangesAsync();
            return Ok();
        }

        return Forbid();
    }
}
