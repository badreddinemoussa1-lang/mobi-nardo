using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using MotoMobiNardo.Data;
using MotoMobiNardo.Models;

namespace MotoMobiNardo.Controllers;

[AllowAnonymous]
public class ShowroomController(MotoMobiDbContext database, IConfiguration configuration) : Controller
{
    [HttpGet]
    public IActionResult Index(string? search, string? brand, int? year, int? minCc, decimal? minPrice, decimal? maxPrice, bool sent = false)
        => BuildCatalog(search, brand, year, minCc, minPrice, maxPrice, new CustomerInquiryInput(), sent);

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("customer-inquiries")]
    public async Task<IActionResult> SubmitInquiry([FromForm(Name = "Inquiry")] CustomerInquiryInput input, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(input.Website))
            return RedirectToAction(nameof(Index), new { sent = true });

        if (!CustomerInquiryTypes.Options.Contains(input.Type, StringComparer.Ordinal))
            ModelState.AddModelError(nameof(input.Type), "Choisissez un type de demande valide.");
        if (input.MotorcycleId.HasValue && !await database.Motorcycles.AnyAsync(m => m.Id == input.MotorcycleId && m.Stock > 0, cancellationToken))
            ModelState.AddModelError(nameof(input.MotorcycleId), "Cette moto n’est plus disponible. Choisissez-en une autre.");
        if (!input.ConsentToContact)
            ModelState.AddModelError(nameof(input.ConsentToContact), "Votre accord est requis pour traiter la demande.");

        if (!ModelState.IsValid)
            return BuildCatalog(null, null, null, null, null, null, input, false);

        database.CustomerInquiries.Add(new CustomerInquiry
        {
            Name = input.Name.Trim(),
            Phone = input.Phone.Trim(),
            Email = string.IsNullOrWhiteSpace(input.Email) ? null : input.Email.Trim(),
            MotorcycleId = input.MotorcycleId,
            Type = input.Type,
            Message = string.IsNullOrWhiteSpace(input.Message) ? null : input.Message.Trim(),
            Status = CustomerInquiryStatuses.New,
            CreatedAtUtc = DateTime.UtcNow
        });
        await database.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Index), new { sent = true });
    }

    private IActionResult BuildCatalog(string? search, string? brand, int? year, int? minCc, decimal? minPrice, decimal? maxPrice, CustomerInquiryInput inquiry, bool sent)
    {
        var available = database.Motorcycles.AsNoTracking().Where(m => m.Stock > 0);
        var brands = available.Select(m => m.Brand).Distinct().OrderBy(x => x).ToList();
        var years = available.Select(m => m.Year).Distinct().OrderByDescending(x => x).ToList();
        var motorcycles = available;
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            motorcycles = motorcycles.Where(m => m.Brand.Contains(term) || m.Model.Contains(term));
        }
        if (!string.IsNullOrWhiteSpace(brand)) motorcycles = motorcycles.Where(m => m.Brand == brand.Trim());
        if (year.HasValue) motorcycles = motorcycles.Where(m => m.Year == year.Value);
        if (minCc.HasValue) motorcycles = motorcycles.Where(m => m.EngineCc >= minCc.Value);
        if (minPrice.HasValue) motorcycles = motorcycles.Where(m => m.Price >= minPrice.Value);
        if (maxPrice.HasValue) motorcycles = motorcycles.Where(m => m.Price <= maxPrice.Value);

        var marketListings = configuration.GetSection("MarketReferences").Get<List<ExternalMarketListing>>() ?? [];
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            marketListings = marketListings.Where(x => x.Title.Contains(term, StringComparison.OrdinalIgnoreCase)
                || (x.Description?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
        }
        if (!string.IsNullOrWhiteSpace(brand))
            marketListings = marketListings.Where(x => x.Title.Contains(brand.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
        if (year.HasValue) marketListings = marketListings.Where(x => x.YearValue == year.Value).ToList();
        if (minCc.HasValue) marketListings = marketListings.Where(x => x.EngineCc.HasValue && x.EngineCc.Value >= minCc.Value).ToList();
        if (minPrice.HasValue) marketListings = marketListings.Where(x => x.PriceValue.HasValue && x.PriceValue.Value >= minPrice.Value).ToList();
        if (maxPrice.HasValue) marketListings = marketListings.Where(x => x.PriceValue.HasValue && x.PriceValue.Value <= maxPrice.Value).ToList();

        var model = new PublicCatalogViewModel
        {
            Business = database.BusinessProfiles.AsNoTracking().SingleOrDefault() ?? configuration.GetSection("BusinessProfile").Get<BusinessProfile>() ?? new BusinessProfile(),
            IsDemo = string.Equals(configuration["MOBI_NARDO_DEMO_MODE"], "true", StringComparison.OrdinalIgnoreCase),
            Inquiry = inquiry,
            InquirySent = sent,
            Motorcycles = motorcycles.OrderBy(m => m.Brand).ThenBy(m => m.Model).ToList(),
            MarketListings = marketListings.OrderByDescending(x => DateTime.TryParseExact(x.Published, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var date) ? date : DateTime.MinValue).ToList(),
            Brands = brands,
            Years = years,
            Search = search?.Trim(),
            Brand = brand?.Trim(),
            Year = year,
            MinCc = minCc,
            MinPrice = minPrice,
            MaxPrice = maxPrice
        };

        return View(model);
    }
}
