using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMobiNardo.Data;
using MotoMobiNardo.Models;

namespace MotoMobiNardo.Controllers;

public class MotorcyclesController(MotoMobiDbContext database, IWebHostEnvironment environment) : Controller
{
    public IActionResult Index(string? search)
    {
        ViewBag.Search = search;
        var query = database.Motorcycles.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(m => m.Brand.Contains(search) || m.Model.Contains(search));
        return View(query.OrderBy(m => m.Brand).ThenBy(m => m.Model).ToList());
    }

    public IActionResult Create() => View(new Motorcycle());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Motorcycle motorcycle, IFormFile? photo, CancellationToken cancellationToken)
    {
        ValidateOptions(motorcycle);
        if (!ModelState.IsValid) return View(motorcycle);
        if (photo is not null)
        {
            motorcycle.PhotoFileName = await SavePhotoAsync(photo, cancellationToken);
            if (!ModelState.IsValid) return View(motorcycle);
        }
        database.Motorcycles.Add(motorcycle);
        database.SaveChanges();
        TempData["Success"] = "La moto a été ajoutée au catalogue.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var motorcycle = database.Motorcycles.Find(id);
        return motorcycle is null ? NotFound() : View(motorcycle);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Motorcycle motorcycle, IFormFile? photo, CancellationToken cancellationToken)
    {
        if (id != motorcycle.Id) return BadRequest();
        if (!ModelState.IsValid) return View(motorcycle);
        var current = database.Motorcycles.Find(id);
        if (current is null) return NotFound();
        ValidateOptions(motorcycle);
        if (!ModelState.IsValid) return View(motorcycle);
        if (photo is not null)
        {
            var replacement = await SavePhotoAsync(photo, cancellationToken);
            if (!ModelState.IsValid) return View(motorcycle);
            current.PhotoFileName = replacement;
        }
        motorcycle.PhotoFileName = current.PhotoFileName;
        current.Brand = motorcycle.Brand;
        current.Model = motorcycle.Model;
        current.Year = motorcycle.Year;
        current.EngineCc = motorcycle.EngineCc;
        current.Price = motorcycle.Price;
        current.Stock = motorcycle.Stock;
        current.Description = motorcycle.Description;
        current.Category = motorcycle.Category;
        current.Condition = motorcycle.Condition;
        try { database.SaveChanges(); }
        catch (DbUpdateConcurrencyException)
        {
            ModelState.AddModelError(nameof(motorcycle.Stock), "Le stock a changé pendant la modification. Rechargez la page et réessayez.");
            return View(motorcycle);
        }
        TempData["Success"] = "Les informations de la moto ont été mises à jour.";
        return RedirectToAction(nameof(Index));
    }

    private void ValidateOptions(Motorcycle motorcycle)
    {
        if (!MotorcycleCategories.Options.Contains(motorcycle.Category, StringComparer.Ordinal))
            ModelState.AddModelError(nameof(motorcycle.Category), "Choisissez une catégorie valide.");
        if (!MotorcycleConditions.Options.Contains(motorcycle.Condition, StringComparer.Ordinal))
            ModelState.AddModelError(nameof(motorcycle.Condition), "Choisissez un état valide.");
    }

    private async Task<string?> SavePhotoAsync(IFormFile photo, CancellationToken cancellationToken)
    {
        const long maxBytes = 5 * 1024 * 1024;
        if (photo.Length == 0 || photo.Length > maxBytes)
        {
            ModelState.AddModelError(nameof(Motorcycle.PhotoFileName), "Choisissez une photo JPG, PNG ou WebP de 5 Mo maximum.");
            return null;
        }

        var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();
        var mime = extension switch { ".jpg" or ".jpeg" => "image/jpeg", ".png" => "image/png", ".webp" => "image/webp", _ => "" };
        if (mime.Length == 0 || photo.ContentType != mime)
        {
            ModelState.AddModelError(nameof(Motorcycle.PhotoFileName), "Format non accepté. Utilisez JPG, PNG ou WebP.");
            return null;
        }

        var header = new byte[12];
        await using (var source = photo.OpenReadStream())
        {
            var count = await source.ReadAsync(header, cancellationToken);
            var valid = extension switch
            {
                ".jpg" or ".jpeg" => count >= 3 && header[0] == 0xff && header[1] == 0xd8 && header[2] == 0xff,
                ".png" => count >= 8 && header.AsSpan(0, 8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
                ".webp" => count >= 12 && System.Text.Encoding.ASCII.GetString(header, 0, 4) == "RIFF" && System.Text.Encoding.ASCII.GetString(header, 8, 4) == "WEBP",
                _ => false
            };
            if (!valid)
            {
                ModelState.AddModelError(nameof(Motorcycle.PhotoFileName), "Le fichier ne semble pas être une image valide.");
                return null;
            }
        }

        var dataDirectory = Environment.GetEnvironmentVariable("MOBI_NARDO_DATA_PATH");
        dataDirectory = string.IsNullOrWhiteSpace(dataDirectory)
            ? Path.Combine(environment.ContentRootPath, "App_Data")
            : Path.GetFullPath(dataDirectory);
        var uploadDirectory = Path.Combine(dataDirectory, "uploads");
        Directory.CreateDirectory(uploadDirectory);
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var destination = Path.Combine(uploadDirectory, fileName);
        await using var output = System.IO.File.Create(destination);
        await photo.CopyToAsync(output, cancellationToken);
        return fileName;
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var motorcycle = database.Motorcycles.Find(id);
        if (motorcycle is null) TempData["Error"] = "Moto introuvable.";
        else if (database.Sales.Any(s => s.MotorcycleId == id)) TempData["Error"] = "Cette moto figure dans l’historique des ventes et ne peut pas être supprimée.";
        else
        {
            database.Motorcycles.Remove(motorcycle);
            database.SaveChanges();
            TempData["Success"] = "La moto a été supprimée.";
        }
        return RedirectToAction(nameof(Index));
    }
}
