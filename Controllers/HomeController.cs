using Microsoft.AspNetCore.Mvc;

namespace NorthwindStore.Controllers;

public class HomeController : Controller
{
    public IActionResult Error()
    {
        return RedirectToAction(nameof(StatusController.Error), "Status");
    }
}
