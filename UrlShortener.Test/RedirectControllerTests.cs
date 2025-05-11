using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Controllers;
using UrlShortener.Models;

namespace UrlShortener.Test;

public class RedirectControllerTests
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
            CreatedDate = DateTime.UtcNow,
            CreatedByUserId = 1
        });

        db.SaveChanges();

        return db;
    }

    [Test]
    public async Task RedirectToOriginal_ShouldRedirect_WhenCodeExists()
    {
        var db = GetDbContext();
        var controller = new RedirectController(db);

        var result = await controller.RedirectToOriginal("a");

        Assert.IsInstanceOf<RedirectResult>(result);
        var redirect = result as RedirectResult;
        Assert.That(redirect!.Url, Is.EqualTo("https://youtube.com"));
    }

    [Test]
    public async Task RedirectToOriginal_ShouldReturnNotFound_WhenCodeMissing()
    {
        var db = GetDbContext();
        var controller = new RedirectController(db);

        var result = await controller.RedirectToOriginal("doesnotexist");

        Assert.IsInstanceOf<NotFoundObjectResult>(result);
        var notFound = result as NotFoundObjectResult;
        Assert.That(notFound!.Value, Is.EqualTo("Short URL not found."));
    }

    [Test]
    public async Task RedirectToOriginal_ShouldReturnBadRequest_WhenCodeEmpty()
    {
        var db = GetDbContext();
        var controller = new RedirectController(db);

        var result = await controller.RedirectToOriginal("");

        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        var badRequest = result as BadRequestObjectResult;
        Assert.That(badRequest!.Value, Is.EqualTo("Short code is required."));
    }
}
