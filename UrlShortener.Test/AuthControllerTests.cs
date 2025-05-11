using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Controllers;
using UrlShortener.Models;

namespace UrlShortener.Test;

public class AuthControllerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Test]
    public async Task Login_ShouldReturnView_WhenCredentialsInvalid()
    {
        var db = GetDbContext();
        db.Users.Add(new User { Login = "admin", PasswordHash = "123", Role = UserRole.User });
        db.SaveChanges();

        var controller = new AuthController(db);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var result = await controller.Login("admin", "wrong");

        Assert.IsInstanceOf<ViewResult>(result);
    }
}
