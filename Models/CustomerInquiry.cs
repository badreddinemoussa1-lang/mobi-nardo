using System.ComponentModel.DataAnnotations;

namespace MotoMobiNardo.Models;

public sealed class CustomerInquiry
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    [Required, StringLength(24)]
    public string Phone { get; set; } = "";

    [EmailAddress, StringLength(254)]
    public string? Email { get; set; }

    public int? MotorcycleId { get; set; }

    [Required, StringLength(32)]
    public string Type { get; set; } = CustomerInquiryTypes.Availability;

    [StringLength(1200)]
    public string? Message { get; set; }

    [Required, StringLength(24)]
    public string Status { get; set; } = CustomerInquiryStatuses.New;

    [StringLength(2000)]
    public string? InternalNote { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}

public sealed class CustomerInquiryInput
{
    [Required, StringLength(100)]
    [Display(Name = "Votre nom")]
    public string Name { get; set; } = "";

    [Required, StringLength(24)]
    [Display(Name = "Téléphone / WhatsApp")]
    public string Phone { get; set; } = "";

    [EmailAddress, StringLength(254)]
    [Display(Name = "E-mail (facultatif)")]
    public string? Email { get; set; }

    [Display(Name = "Moto concernée")]
    public int? MotorcycleId { get; set; }

    [Required, StringLength(32)]
    [Display(Name = "Votre demande")]
    public string Type { get; set; } = CustomerInquiryTypes.Availability;

    [StringLength(1200)]
    [Display(Name = "Message (facultatif)")]
    public string? Message { get; set; }

    [Display(Name = "J’accepte d’être recontacté au sujet de cette demande.")]
    public bool ConsentToContact { get; set; }

    [StringLength(120)]
    public string? Website { get; set; }
}

public static class CustomerInquiryTypes
{
    public const string Availability = "Disponibilité";
    public const string Price = "Prix / devis";
    public const string Callback = "Rappel";
    public const string Visit = "Visite showroom";
    public const string Financing = "Financement";
    public const string Reservation = "Demande de réservation";

    public static IReadOnlyList<string> Options { get; } = [Availability, Price, Callback, Visit, Financing, Reservation];
}

public static class CustomerInquiryStatuses
{
    public const string New = "Nouveau";
    public const string Contacted = "Contacté";
    public const string Interested = "Intéressé";
    public const string Reserved = "Réservé";
    public const string Won = "Vente conclue";
    public const string Lost = "Perdu";

    public static IReadOnlyList<string> Options { get; } = [New, Contacted, Interested, Reserved, Won, Lost];
}

public sealed class CustomerInquiriesViewModel
{
    public IReadOnlyList<CustomerInquiry> Inquiries { get; init; } = [];
    public IReadOnlyDictionary<int, string> MotorcycleNames { get; init; } = new Dictionary<int, string>();
}
