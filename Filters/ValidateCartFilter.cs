using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NorthwindStore.Interfaces;

namespace NorthwindStore.Filters;

public class ValidateCartFilter(ICartService cart) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (cart.GetCart().IsEmpty)
        {
            context.Result = new RedirectToActionResult("Index", "Cart", new { message = "Cart is empty." });
            return;
        }

        await next();
    }
}
