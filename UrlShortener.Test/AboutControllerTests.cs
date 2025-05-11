using Microsoft.EntityFrameworkCore;
using UrlShortener.Controllers;
using UrlShortener.Models;
using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.Test;
public class AboutControllerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new AppDbContext(options);
        db.AboutInfos.Add(new AboutInfo { Id = 1, Description = "Initial" });
        db.SaveChanges();
        return db;
    }

    [Test]
    public async Task Edit_ShouldUpdateDescription()
    {
        var db = GetDbContext();
        var controller = new AboutController(db);
        var newDescription = "Updated description";

        var result = await controller.Edit(newDescription);
        var updated = await db.AboutInfos.FirstOrDefaultAsync();

        Assert.IsInstanceOf<RedirectToActionResult>(result);
        Assert.That(updated!.Description, Is.EqualTo(newDescription));
    }


    [Test]
    public async Task Index_ShouldCreateDefaultDescription_IfMissing()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new AppDbContext(options);
        var controller = new AboutController(db);
        var result = await controller.Index();

        var about = await db.AboutInfos.FirstOrDefaultAsync();
        Assert.IsNotNull(about);
        Assert.That(about!.Description, Does.Contain("URL Shortener"));

        var view = result as ViewResult;
        var model = view!.Model as AboutInfo;
        Assert.That(model!.Description, Is.EqualTo(about.Description));
    }

    [Test]
    public async Task Index_ShouldReturnView_WhenDescriptionExists()
    {
        var db = GetDbContext();
        var controller = new AboutController(db);
        var result = await controller.Index();

        Assert.IsInstanceOf<ViewResult>(result);
        var view = result as ViewResult;
        var model = view!.Model as AboutInfo;

        Assert.IsNotNull(model);
        Assert.That(model!.Description, Is.EqualTo("Initial"));
    }
}

