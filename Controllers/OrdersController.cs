using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NorthwindStore.Helpers;
using NorthwindStore.Interfaces;
using NorthwindStore.Models.Identity;

namespace NorthwindStore.Controllers;

[Authorize(Roles = RoleNames.Customer)]
public class OrdersController(IOrderService orders, UserManager<ApplicationUser> users) : Controller
{
    public async Task<IActionResult> MyOrders(int page = 1)
    {
        var user = await users.GetUserAsync(User);
        if (user?.CustomerId is null)
        {
            return Challenge();
        }

        return View(await orders.GetCustomerOrdersAsync(user.CustomerId, page));
    }

    public async Task<IActionResult> Details(int id)
    {
        var user = await users.GetUserAsync(User);
        var model = await orders.GetOrderDetailsAsync(id, user?.CustomerId, allowAnyCustomer: false);
        return model is null ? NotFound() : View(model);
    }

    public async Task<IActionResult> Summary(int id)
    {
        var user = await users.GetUserAsync(User);
        var model = await orders.GetOrderDetailsAsync(id, user?.CustomerId, allowAnyCustomer: false);
        return model is null ? NotFound() : View(model);
    }
}
