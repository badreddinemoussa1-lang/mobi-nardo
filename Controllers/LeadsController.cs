using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMobiNardo.Data;
using MotoMobiNardo.Models;

namespace MotoMobiNardo.Controllers;

public sealed class LeadsController(MotoMobiDbContext database) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var inquiries = await database.CustomerInquiries.AsNoTracking()
            .OrderBy(i => i.Status == CustomerInquiryStatuses.Won || i.Status == CustomerInquiryStatuses.Lost)
            .ThenByDescending(i => i.CreatedAtUtc)
            .Take(250)
            .ToListAsync(cancellationToken);
        var ids = inquiries.Where(i => i.MotorcycleId.HasValue).Select(i => i.MotorcycleId!.Value).Distinct().ToArray();
        var motorcycles = await database.Motorcycles.AsNoTracking().Where(m => ids.Contains(m.Id))
            .ToDictionaryAsync(m => m.Id, m => m.FullName, cancellationToken);
        return View(new CustomerInquiriesViewModel { Inquiries = inquiries, MotorcycleNames = motorcycles });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, string status, string? internalNote, CancellationToken cancellationToken)
    {
        if (!CustomerInquiryStatuses.Options.Contains(status, StringComparer.Ordinal))
        {
            TempData["Error"] = "Choisissez une étape valide pour cette demande.";
            return RedirectToAction(nameof(Index));
        }

        var inquiry = await database.CustomerInquiries.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        if (inquiry is null) return NotFound();
        inquiry.Status = status;
        inquiry.InternalNote = string.IsNullOrWhiteSpace(internalNote) ? null : internalNote.Trim();
        inquiry.UpdatedAtUtc = DateTime.UtcNow;
        await database.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Le suivi de la demande a été mis à jour.";
        return RedirectToAction(nameof(Index));
    }
}
