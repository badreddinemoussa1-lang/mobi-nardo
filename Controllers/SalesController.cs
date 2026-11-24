using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MotoMobiNardo.Data;
using MotoMobiNardo.Models;

namespace MotoMobiNardo.Controllers;

public class SalesController(MotoMobiDbContext database, IConfiguration configuration) : Controller
{
    public IActionResult Index()
    {
        var sales = database.Sales.AsNoTracking().OrderByDescending(s => s.SoldAt).ThenByDescending(s => s.Id).ToList();
        var motorcycleNames = database.Motorcycles.AsNoTracking().ToDictionary(m => m.Id, m => m.FullName);
        var customerNames = database.Customers.AsNoTracking().ToDictionary(c => c.Id, c => c.FullName);
        return View(sales.Select(s => (Sale: s,
            Motorcycle: motorcycleNames.GetValueOrDefault(s.MotorcycleId, "Moto indisponible"),
            Customer: customerNames.GetValueOrDefault(s.CustomerId, "Client indisponible"))).ToList());
    }

    public IActionResult Create()
    {
        PopulateOptions();
        return View(new Sale { SoldAt = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(Sale sale)
    {
        if (!SalesChannels.Options.Contains(sale.Channel, StringComparer.Ordinal))
            ModelState.AddModelError(nameof(sale.Channel), "Choisissez un canal de vente valide.");

        var motorcycle = database.Motorcycles.Find(sale.MotorcycleId);
        var customerExists = database.Customers.Any(c => c.Id == sale.CustomerId);
        if (motorcycle is null || !customerExists)
            ModelState.AddModelError("", "Choisissez une moto et un client valides.");
        else
        {
            if (sale.Quantity > motorcycle.Stock)
                ModelState.AddModelError(nameof(sale.Quantity), $"Stock disponible : {motorcycle.Stock} unité(s).");
            if (sale.Discount > motorcycle.Price * sale.Quantity)
                ModelState.AddModelError(nameof(sale.Discount), "La remise dépasse le montant de la vente.");
        }

        if (!ModelState.IsValid || motorcycle is null)
        {
            PopulateOptions(sale.MotorcycleId, sale.CustomerId, sale.Channel);
            return View(sale);
        }

        sale.UnitPrice = motorcycle.Price;
        motorcycle.Stock -= sale.Quantity;
        database.Sales.Add(sale);
        try { database.SaveChanges(); }
        catch (DbUpdateConcurrencyException)
        {
            ModelState.AddModelError(nameof(sale.Quantity), "Le stock vient d’être modifié. Vérifiez la disponibilité puis réessayez.");
            PopulateOptions(sale.MotorcycleId, sale.CustomerId, sale.Channel);
            return View(sale);
        }
        TempData["Success"] = $"La vente {sale.Reference} a été enregistrée.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Invoice(int id)
    {
        var sale = database.Sales.AsNoTracking().FirstOrDefault(s => s.Id == id);
        if (sale is null) return NotFound();
        var motorcycle = database.Motorcycles.AsNoTracking().FirstOrDefault(m => m.Id == sale.MotorcycleId);
        var customer = database.Customers.AsNoTracking().FirstOrDefault(c => c.Id == sale.CustomerId);
        if (motorcycle is null || customer is null) return NotFound();
        var business = database.BusinessProfiles.AsNoTracking().SingleOrDefault() ?? configuration.GetSection("BusinessProfile").Get<BusinessProfile>() ?? new BusinessProfile();
        return View(new SaleInvoiceViewModel { Sale = sale, Motorcycle = motorcycle, Customer = customer, Business = business });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Cancel(int id)
    {
        var sale = database.Sales.FirstOrDefault(s => s.Id == id);
        if (sale is null) TempData["Error"] = "Vente introuvable.";
        else if (sale.IsCancelled) TempData["Error"] = "Cette vente est déjà annulée.";
        else
        {
            var motorcycle = database.Motorcycles.Find(sale.MotorcycleId);
            sale.IsCancelled = true;
            sale.CancelledAt = DateTime.Now;
            if (motorcycle is not null) motorcycle.Stock += sale.Quantity;
            database.SaveChanges();
            TempData["Success"] = $"La vente {sale.Reference} a été annulée. Le stock a été restitué.";
        }
        return RedirectToAction(nameof(Index));
    }

    private void PopulateOptions(int? motorcycleId = null, int? customerId = null, string? channel = null)
    {
        var motorcycles = database.Motorcycles.AsNoTracking().Where(m => m.Stock > 0).OrderBy(m => m.Brand).ToList();
        var customers = database.Customers.AsNoTracking().OrderBy(c => c.FullName).ToList();
        ViewBag.Motorcycles = new SelectList(motorcycles.Select(m => new { m.Id, Label = $"{m.Brand} {m.Model} · {m.Price:N0} DH · stock {m.Stock}" }), "Id", "Label", motorcycleId);
        ViewBag.Customers = new SelectList(customers.Select(c => new { c.Id, Label = $"{c.FullName} · {c.Phone}" }), "Id", "Label", customerId);
        ViewBag.Channels = new SelectList(SalesChannels.Options, channel ?? SalesChannels.Showroom);
        ViewBag.HasMotorcycles = motorcycles.Count > 0;
        ViewBag.HasCustomers = customers.Count > 0;
    }
}
