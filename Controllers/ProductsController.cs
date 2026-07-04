using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindStore.Infrastructure;
using NorthwindStore.Services;

namespace NorthwindStore.Controllers;

[Authorize(Roles = RoleNames.Customer)]
public class ProductsController(IProductService products) : Controller
{
    public async Task<IActionResult> Index(string? search, string? sort, int page = 1)
    {
        return View(await products.GetAvailableProductsAsync(search, sort, page));
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await products.GetProductAsync(id);
        return product is null ? NotFound() : View(product);
    }
}
