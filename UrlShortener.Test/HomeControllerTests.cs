using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UrlShortener.Controllers;
using UrlShortener.Models;

namespace UrlShortener.Test;

public class HomeControllerTests
{
    private HomeController GetController()
    {
        var logger = new LoggerFactory().CreateLogger<HomeController>();
        var controller = new HomeController(logger);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        return controller;
    }

    [Test]
    public void Index_ShouldReturnView()
    {
        var controller = GetController();

        var result = controller.Index();

        Assert.IsInstanceOf<ViewResult>(result);
    }

    [Test]
    public void Privacy_ShouldReturnView()
    {
        var controller = GetController();

        var result = controller.Privacy();

        Assert.IsInstanceOf<ViewResult>(result);
    }

    [Test]
    public void Error_ShouldReturnViewWithModel()
    {
        var controller = GetController();

        var result = controller.Error();

        Assert.IsInstanceOf<ViewResult>(result);

        var viewResult = result as ViewResult;
        Assert.IsInstanceOf<ErrorViewModel>(viewResult!.Model);
        var model = viewResult.Model as ErrorViewModel;

        Assert.That(model!.RequestId, Is.Not.Null.Or.Empty);
    }
}
