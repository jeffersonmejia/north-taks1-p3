using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindStore.Helpers;
using NorthwindStore.Interfaces;

namespace NorthwindStore.Controllers;

[Authorize(Roles = RoleNames.Admin)]
public class AdminOrdersController(IOrderService orders) : Controller
{
    public async Task<IActionResult> Orders(int page = 1)
    {
        return View("~/Views/Admin/Orders.cshtml", await orders.GetAllOrdersAsync(page));
    }

    public async Task<IActionResult> Details(int id)
    {
        var model = await orders.GetOrderDetailsAsync(id, customerId: null, allowAnyCustomer: true);
        return model is null ? NotFound() : View("~/Views/Orders/Details.cshtml", model);
    }
}
