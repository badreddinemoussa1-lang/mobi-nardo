using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MotoMobiNardo.Models;
using System.Security.Cryptography;
using System.Text;

namespace MotoMobiNardo.Controllers;

[AllowAnonymous]
public class AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IConfiguration configuration, IWebHostEnvironment environment) : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
        if (!userManager.Users.Any()) return RedirectToAction(nameof(Setup));
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
        if (result.Succeeded)
            return Url.IsLocalUrl(model.ReturnUrl) ? Redirect(model.ReturnUrl!) : RedirectToAction("Index", "Home");
        ModelState.AddModelError("", result.IsLockedOut
            ? "Trop de tentatives. Réessayez dans quelques minutes."
            : "E-mail ou mot de passe incorrect.");
        return View(model);
    }

    [HttpGet]
    public IActionResult Setup(string? key)
    {
        if (userManager.Users.Any()) return RedirectToAction(nameof(Login));
        if (!IsBootstrapKeyValid(key)) return NotFound();
        return View(new SetupViewModel { BootstrapKey = key });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Setup(SetupViewModel model)
    {
        if (userManager.Users.Any()) return RedirectToAction(nameof(Login));
        if (!IsBootstrapKeyValid(model.BootstrapKey)) return NotFound();
        if (!ModelState.IsValid) return View(model);
        var user = new IdentityUser { UserName = model.Email.Trim(), Email = model.Email.Trim(), EmailConfirmed = true };
        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
            return View(model);
        }
        await signInManager.SignInAsync(user, isPersistent: false);
        TempData["Success"] = "Le compte administrateur a été créé.";
        return RedirectToAction("Index", "Home");
    }

    private bool IsBootstrapKeyValid(string? provided)
    {
        var expected = configuration["MOBI_NARDO_BOOTSTRAP_KEY"];
        if (environment.IsDevelopment() && string.IsNullOrEmpty(expected)) return true;
        if (string.IsNullOrEmpty(expected) || string.IsNullOrEmpty(provided)) return false;
        var expectedBytes = Encoding.UTF8.GetBytes(expected);
        var providedBytes = Encoding.UTF8.GetBytes(provided);
        return expectedBytes.Length == providedBytes.Length && CryptographicOperations.FixedTimeEquals(expectedBytes, providedBytes);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }
}
