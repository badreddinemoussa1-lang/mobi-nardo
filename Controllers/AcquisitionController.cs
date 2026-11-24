using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MotoMobiNardo.Data;
using MotoMobiNardo.Models;

namespace MotoMobiNardo.Controllers;

[AllowAnonymous]
[Route("acquisition")]
public sealed class AcquisitionController(MotoMobiDbContext database) : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View(new AcquisitionInquiryInput());

    [HttpPost("inquire")]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("acquisition-inquiries")]
    public async Task<IActionResult> Inquire(AcquisitionInquiryInput input, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View("Index", input);

        database.AcquisitionInquiries.Add(new AcquisitionInquiry
        {
            Name = input.Name.Trim(),
            Email = input.Email.Trim(),
            Company = string.IsNullOrWhiteSpace(input.Company) ? null : input.Company.Trim(),
            Message = input.Message.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        });
        await database.SaveChangesAsync(cancellationToken);
        TempData["InquirySent"] = true;
        return RedirectToAction(nameof(Index));
    }
}
