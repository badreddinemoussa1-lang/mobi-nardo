using System.ComponentModel.DataAnnotations;

namespace MotoMobiNardo.Models;

public sealed class AcquisitionInquiry
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = "";

    [StringLength(120)]
    public string? Company { get; set; }

    [Required, StringLength(2000, MinimumLength = 20)]
    public string Message { get; set; } = "";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public sealed class AcquisitionInquiryInput
{
    [Required, StringLength(100)]
    [Display(Name = "Votre nom")]
    public string Name { get; set; } = "";

    [Required, EmailAddress, StringLength(254)]
    [Display(Name = "Votre e-mail")]
    public string Email { get; set; } = "";

    [StringLength(120)]
    [Display(Name = "Entreprise (facultatif)")]
    public string? Company { get; set; }

    [Required, StringLength(2000, MinimumLength = 20)]
    [Display(Name = "Votre message")]
    public string Message { get; set; } = "";
}
