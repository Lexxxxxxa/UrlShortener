using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using UrlShortener.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace UrlShortener.Controllers;

public class AuthController : Controller
{
    private readonly AppDbContext _db;

    public AuthController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string login, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Login == login && u.PasswordHash == password);
        if (user == null)
        {
            ViewBag.Error = "Invalid credentials";
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");

        await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(claimsIdentity));

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "Login and password are required.";
            return View();
        }

        if (_db.Users.Any(u => u.Login == login))
        {
            ViewBag.Error = "User already exists.";
            return View();
        }

        var user = new User
        {
            Login = login,
            PasswordHash = password,
            Role = UserRole.User
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var claims = new List<Claim>
        {
        new Claim(ClaimTypes.Name, user.Login),
        new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");

        await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(claimsIdentity));

        return RedirectToAction("Index", "Home");
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> DeleteAccount()
    {
        var login = User.Identity?.Name;
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Login == login);

        if (user != null)
        {
            var urls = _db.ShortUrls.Where(s => s.CreatedByUserId == user.Id);
            _db.ShortUrls.RemoveRange(urls);

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            await HttpContext.SignOutAsync("MyCookieAuth");
        }

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("MyCookieAuth");
        return RedirectToAction("Index", "Home");
    }
}
