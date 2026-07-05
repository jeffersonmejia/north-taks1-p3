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
    public async Task<IActionResult> InventoryPartial(string filter = "all", int page = 1)
    {
        var model = filter switch
        {
            "low" => await inventory.GetLowStockAsync(page),
            "out" => await inventory.GetOutOfStockAsync(page),
            "discontinued" => await inventory.GetDiscontinuedAsync(page),
            _ => await inventory.GetInventoryAsync(page)
        };

        return PartialView("~/Views/Admin/_InventoryList.cshtml", model);
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

    [HttpPost]
    public async Task<IActionResult> ToggleDiscontinued(int productId)
    {
        var (success, message, newValue) = await inventory.ToggleDiscontinuedAsync(productId);
        return Json(new { success, message, newValue });
    }

    [HttpPost]
    public async Task<IActionResult> QuickAdjust(int productId, string operation)
    {
        var model = await inventory.GetStockUpdateAsync(productId, operation);
        if (model is null)
            return Json(new { success = false });

        model.Quantity = 1;
        var result = await inventory.UpdateStockAsync(model, User.Identity?.Name ?? "admin");

        return Json(new
        {
            success = result.Success,
            message = result.Message,
            newStock = result.Model?.NewStock,
            productName = result.Model?.ProductName
        });
    }
}
