using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMobiNardo.Data;

namespace MotoMobiNardo.Controllers;

[AllowAnonymous]
public sealed class MediaController(MotoMobiDbContext database, IWebHostEnvironment environment) : Controller
{
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Photo(int id, CancellationToken cancellationToken)
    {
        var photo = await database.Motorcycles.AsNoTracking().Where(m => m.Id == id)
            .Select(m => new
            {
                m.PhotoFileName,
                Data = m.Photo == null ? null : m.Photo.Data,
                ContentType = m.Photo == null ? null : m.Photo.ContentType
            }).FirstOrDefaultAsync(cancellationToken);
        var fileName = photo?.PhotoFileName;
        if (string.IsNullOrWhiteSpace(fileName) || Path.GetFileName(fileName) != fileName) return NotFound();
        if (photo?.Data is { Length: > 0 } imageData && photo.ContentType is ("image/jpeg" or "image/png" or "image/webp"))
            return File(imageData, photo.ContentType);
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var contentType = extension switch { ".jpg" or ".jpeg" => "image/jpeg", ".png" => "image/png", ".webp" => "image/webp", _ => null };
        if (contentType is null) return NotFound();

        var dataDirectory = Environment.GetEnvironmentVariable("MOBI_NARDO_DATA_PATH");
        dataDirectory = string.IsNullOrWhiteSpace(dataDirectory)
            ? Path.Combine(environment.ContentRootPath, "App_Data")
            : Path.GetFullPath(dataDirectory);
        var path = Path.Combine(dataDirectory, "uploads", fileName);
        return System.IO.File.Exists(path) ? PhysicalFile(path, contentType) : NotFound();
    }
}
