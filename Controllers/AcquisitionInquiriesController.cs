using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMobiNardo.Data;

namespace MotoMobiNardo.Controllers;

public sealed class AcquisitionInquiriesController(MotoMobiDbContext database) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var inquiries = await database.AcquisitionInquiries
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(100)
            .ToListAsync(cancellationToken);
        return View(inquiries);
    }
}
