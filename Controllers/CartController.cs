using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindStore.Infrastructure;
using NorthwindStore.Repositories;
using NorthwindStore.Services;
using System.Security.Claims;

namespace NorthwindStore.Controllers;

[Authorize(Roles = RoleNames.Customer)]
public class CartController(ICartService cart, IOrderService orders) : Controller
{
    public IActionResult Index(string? message = null)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            TempData["Error"] = message;
        }
        return View(cart.GetCart());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity)
    {
        var result = await cart.AddAsync(productId, quantity);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction("Index", "Products");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int productId, int quantity)
    {
        var result = await cart.UpdateAsync(productId, quantity);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int productId)
    {
        cart.Remove(productId);
        TempData["Success"] = "Product removed.";
        return RedirectToAction(nameof(Index));
    }

    [ServiceFilter(typeof(ValidateCartFilter))]
    public IActionResult Confirm() => View(cart.GetCart());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ServiceFilter(typeof(ValidateCartFilter))]
    public async Task<IActionResult> ConfirmPurchase()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Challenge();
        }

        var result = await orders.ConfirmCartAsync(userId);
        if (!result.Success)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction("Summary", "Orders", new { id = result.OrderId });
    }
}
