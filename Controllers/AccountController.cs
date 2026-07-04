using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NorthwindStore.Data;
using NorthwindStore.Infrastructure;
using NorthwindStore.Repositories;
using NorthwindStore.Services;
using NorthwindStore.Models.Identity;
using NorthwindStore.Models.Northwind;
using NorthwindStore.Models.ViewModels.Auth;
using System.Security.Claims;

namespace NorthwindStore.Controllers;

public class AccountController(
    UserManager<ApplicationUser> users,
    SignInManager<ApplicationUser> signIn,
    ISessionControlService sessions,
    NorthwindContext northwind) : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(bool expired = false)
    {
        if (expired)
        {
            TempData["Error"] = "Your session was closed because the account signed in somewhere else.";
        }
        return View(new LoginViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await users.FindByEmailAsync(model.Email);
        if (user is null || !await users.CheckPasswordAsync(user, model.Password))
        {
            ModelState.AddModelError(string.Empty, "Invalid credentials.");
            return View(model);
        }

        var sessionId = await sessions.StartSessionAsync(user);
        var principal = await signIn.CreateUserPrincipalAsync(user);
        ((ClaimsIdentity)principal.Identity!).AddClaim(new Claim(Constants.SessionClaimType, sessionId));
        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal, new AuthenticationProperties
        {
            IsPersistent = model.RememberMe
        });

        return RedirectToAction("Index", "Products");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var customerId = Guid.NewGuid().ToString("N")[..5].ToUpperInvariant();
        northwind.Customers.Add(new Customer
        {
            CustomerId = customerId,
            CompanyName = model.FullName,
            ContactName = model.FullName,
            ContactTitle = "Online Customer",
            City = "Online"
        });
        await northwind.SaveChangesAsync();

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = true,
            FullName = model.FullName,
            CustomerId = customerId
        };

        var result = await users.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        await users.AddToRoleAsync(user, RoleNames.Customer);
        return RedirectToAction(nameof(Login));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var user = await users.GetUserAsync(User);
        if (user is not null)
        {
            await sessions.EndSessionAsync(user);
        }
        await signIn.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    public IActionResult AccessDenied() => View();
}
