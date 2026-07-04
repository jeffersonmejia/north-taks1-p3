using Microsoft.AspNetCore.Mvc;

namespace NorthwindStore.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Error()
    {
        return RedirectToAction(nameof(StatusController.Error), "Status");
    }
}
