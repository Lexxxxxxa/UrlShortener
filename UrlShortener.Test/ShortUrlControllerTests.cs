using UrlShortener.Controllers;
using UrlShortener.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.Test;
public class ShortUrlControllerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new AppDbContext(options);
        db.ShortUrls.Add(new ShortUrl
        {
            Id = 1,
            OriginalUrl = "https://youtube.com",
            ShortCode = "a",
            CreatedByUserId = 1
        });
        db.SaveChanges();
        return db;
    }

    [Test]
    public async Task AddShouldReturnBadRequestIfDuplicateUrl()
    {
        var db = GetDbContext();
        var controller = new ShortUrlController(db);
        string duplicateUrl = "https://youtube.com";

        var result = await controller.Add(duplicateUrl);

        Assert.IsInstanceOf<BadRequestObjectResult>(result);

        var badRequest = result as BadRequestObjectResult;

        var json = System.Text.Json.JsonSerializer.Serialize(badRequest!.Value);
        var expected = System.Text.Json.JsonSerializer.Serialize(new { message = "This URL already exists." });

        Assert.That(json, Is.EqualTo(expected));
    }

    [Test]
    public async Task Delete_ShouldReturnNotFound_IfNotExists()
    {
        var db = GetDbContext();
        var controller = new ShortUrlController(db);
        int nonexistentId = 666;

        var result = await controller.Delete(nonexistentId);

        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task Delete_ShouldReturnNotFound_IfUrlIsNull()
    {
        var db = GetDbContext();
        var controller = new ShortUrlController(db);
        int nonexistentId = 666;

        var result = await controller.Delete(nonexistentId);

        Assert.IsInstanceOf<NotFoundResult>(result);
    }
}
