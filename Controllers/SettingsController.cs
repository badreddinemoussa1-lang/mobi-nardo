using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMobiNardo.Data;
using MotoMobiNardo.Models;

namespace MotoMobiNardo.Controllers;

public sealed class SettingsController(MotoMobiDbContext database, IConfiguration configuration) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var profile = await database.BusinessProfiles.SingleOrDefaultAsync(cancellationToken)
            ?? configuration.GetSection("BusinessProfile").Get<BusinessProfile>()
            ?? new BusinessProfile();
        return View(profile);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(BusinessProfile input, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(input);

        ValidateHttpsUrls(input);
        if (!ModelState.IsValid) return View(input);

        var profile = await database.BusinessProfiles.SingleOrDefaultAsync(cancellationToken);
        if (profile is null)
        {
            profile = configuration.GetSection("BusinessProfile").Get<BusinessProfile>() ?? new BusinessProfile();
            profile.Id = 1;
            database.BusinessProfiles.Add(profile);
        }

        profile.Name = input.Name.Trim();
        profile.City = input.City.Trim();
        profile.Address = Clean(input.Address);
        profile.Phone = Clean(input.Phone);
        profile.WhatsApp = Clean(input.WhatsApp);
        profile.Email = Clean(input.Email);
        profile.LogoUrl = Clean(input.LogoUrl);
        profile.PrimaryColor = input.PrimaryColor;
        profile.SecondaryColor = input.SecondaryColor;
        profile.GoogleMapsUrl = Clean(input.GoogleMapsUrl);
        profile.OpeningHours = Clean(input.OpeningHours);
        profile.InstagramUrl = Clean(input.InstagramUrl);
        profile.TikTokUrl = Clean(input.TikTokUrl);
        profile.FacebookUrl = Clean(input.FacebookUrl);
        profile.OfficialWebsiteUrl = Clean(input.OfficialWebsiteUrl);
        profile.LiveCatalogUrl = Clean(input.LiveCatalogUrl);
        profile.Currency = input.Currency.Trim();
        await database.SaveChangesAsync(cancellationToken);

        TempData["Success"] = "Le profil du showroom a été mis à jour.";
        return RedirectToAction(nameof(Index));
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private void ValidateHttpsUrls(BusinessProfile input)
    {
        var fields = new (string Name, string? Value)[]
        {
            (nameof(input.LogoUrl), input.LogoUrl),
            (nameof(input.GoogleMapsUrl), input.GoogleMapsUrl),
            (nameof(input.InstagramUrl), input.InstagramUrl),
            (nameof(input.TikTokUrl), input.TikTokUrl),
            (nameof(input.FacebookUrl), input.FacebookUrl),
            (nameof(input.OfficialWebsiteUrl), input.OfficialWebsiteUrl),
            (nameof(input.LiveCatalogUrl), input.LiveCatalogUrl)
        };

        foreach (var field in fields)
        {
            if (!string.IsNullOrWhiteSpace(field.Value) &&
                (!Uri.TryCreate(field.Value, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps))
                ModelState.AddModelError(field.Name, "Utilisez une URL complète commençant par https://.");
        }
    }
}
