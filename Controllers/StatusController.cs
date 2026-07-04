using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace NorthwindStore.Controllers;

public class StatusController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var isDbError = HttpContext.Items["DbError"] is true;
        var statusCode = HttpContext.Items["StatusCode"] is int code ? code : 500;

        HttpContext.Response.StatusCode = statusCode;

        if (isDbError)
        {
            return View("~/Views/Shared/Error.cshtml", new ErrorViewModel
            {
                TraceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                IsDbError = true,
                Message = "The service is temporarily unavailable. Please try again later."
            });
        }

        return View("~/Views/Shared/Error.cshtml", new ErrorViewModel
        {
            TraceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            IsDbError = false,
            Message = "An unexpected error occurred. Please try again later."
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Unavailable()
    {
        HttpContext.Response.StatusCode = 503;

        return View("~/Views/Shared/Error.cshtml", new ErrorViewModel
        {
            TraceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            IsDbError = true,
                Message = "The service is temporarily unavailable. Please try again later."
        });
    }
}

public class ErrorViewModel
{
    public required string TraceId { get; set; }
    public bool IsDbError { get; set; }
    public required string Message { get; set; }
}
