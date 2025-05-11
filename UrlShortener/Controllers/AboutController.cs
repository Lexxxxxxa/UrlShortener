using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Models;

namespace UrlShortener.Controllers;

public class AboutController : Controller
{
    private readonly AppDbContext _db;

    public AboutController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var about = await _db.AboutInfos.FirstOrDefaultAsync();
        if (about == null)
        {
            about = new AboutInfo { Description = "This is a URL Shortener project made by Oleksii Zubtsov." };
            _db.AboutInfos.Add(about);
            await _db.SaveChangesAsync();
        }

        return View(about);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Edit(string description)
    {
        var about = await _db.AboutInfos.FirstOrDefaultAsync();
        if (about != null)
        {
            about.Description = description;
            _db.AboutInfos.Update(about);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
}
