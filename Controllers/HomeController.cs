using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace NorthwindStore.Controllers;

public class HomeController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("~/Views/Shared/Error.cshtml", Activity.Current?.Id ?? HttpContext.TraceIdentifier);
    }
}
