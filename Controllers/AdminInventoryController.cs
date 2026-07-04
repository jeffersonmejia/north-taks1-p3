using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindStore.Infrastructure;
using NorthwindStore.Repositories;
using NorthwindStore.Services;
using NorthwindStore.Models.ViewModels.Admin;

namespace NorthwindStore.Controllers;

[Authorize(Roles = RoleNames.Admin)]
public class AdminInventoryController(IInventoryService inventory) : Controller
{
    public async Task<IActionResult> Inventory(int page = 1)
    {
        return View("~/Views/Admin/Inventory.cshtml", await inventory.GetInventoryAsync(page));
    }

    public async Task<IActionResult> LowStock(int page = 1)
    {
        return View("~/Views/Admin/LowStock.cshtml", await inventory.GetLowStockAsync(page));
    }

    public async Task<IActionResult> OutOfStock(int page = 1)
    {
        return View("~/Views/Admin/OutOfStock.cshtml", await inventory.GetOutOfStockAsync(page));
    }

    public async Task<IActionResult> Discontinued(int page = 1)
    {
        return View("~/Views/Admin/Discontinued.cshtml", await inventory.GetDiscontinuedAsync(page));
    }

    [HttpGet]
    public async Task<IActionResult> UpdateStock(int productId, string operation = "increase")
    {
        var model = await inventory.GetStockUpdateAsync(productId, operation);
        return model is null ? NotFound() : View("~/Views/Admin/UpdateStock.cshtml", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStock(StockUpdateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Admin/UpdateStock.cshtml", model);
        }

        var result = await inventory.UpdateStockAsync(model, User.Identity?.Name ?? "admin");
        if (!result.Success)
        {
            TempData["Error"] = result.Message;
            return result.Model is null ? RedirectToAction(nameof(Inventory)) : View("~/Views/Admin/UpdateStock.cshtml", result.Model);
        }

        TempData["Success"] = result.Message;
        return View("~/Views/Admin/UpdateStock.cshtml", result.Model);
    }
}
