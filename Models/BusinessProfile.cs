using System.ComponentModel.DataAnnotations;

namespace MotoMobiNardo.Models;

public class BusinessProfile
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Nom de l’établissement")]
    public string Name { get; set; } = "Moto Mobi Nardo";

    [Required, StringLength(80)]
    [Display(Name = "Ville")]
    public string City { get; set; } = "Bouskoura";

    [StringLength(240)]
    [Display(Name = "Adresse")]
    public string? Address { get; set; }

    [Phone, StringLength(24)]
    [Display(Name = "Téléphone")]
    public string? Phone { get; set; }

    [Phone, StringLength(24)]
    [Display(Name = "WhatsApp")]
    public string? WhatsApp { get; set; }

    [EmailAddress, StringLength(254)]
    [Display(Name = "E-mail")]
    public string? Email { get; set; }

    [Url, StringLength(500)]
    [Display(Name = "URL du logo")]
    public string? LogoUrl { get; set; }

    [Required, RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Indiquez une couleur hexadécimale, par exemple #ff6b00.")]
    [Display(Name = "Couleur principale")]
    public string PrimaryColor { get; set; } = "#ff6b00";

    [Required, RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Indiquez une couleur hexadécimale, par exemple #080808.")]
    [Display(Name = "Couleur secondaire")]
    public string SecondaryColor { get; set; } = "#080808";

    [Url, StringLength(500)]
    [Display(Name = "Lien Google Maps")]
    public string? GoogleMapsUrl { get; set; }

    [StringLength(500)]
    [Display(Name = "Horaires d’ouverture")]
    public string? OpeningHours { get; set; }

    [Url, StringLength(500)]
    [Display(Name = "Instagram")]
    public string? InstagramUrl { get; set; }

    [Url, StringLength(500)]
    [Display(Name = "TikTok")]
    public string? TikTokUrl { get; set; }

    [Url, StringLength(500)]
    [Display(Name = "Facebook")]
    public string? FacebookUrl { get; set; }

    [Url, StringLength(500)]
    [Display(Name = "Site officiel")]
    public string? OfficialWebsiteUrl { get; set; }

    [Url, StringLength(500)]
    [Display(Name = "Catalogue hébergé existant")]
    public string? LiveCatalogUrl { get; set; }

    [Required, StringLength(8)]
    [Display(Name = "Devise")]
    public string Currency { get; set; } = "DH";

    [StringLength(80)]
    public string? FiscalIdentifier { get; set; }
    [StringLength(80)]
    public string? ProfessionalTaxNumber { get; set; }
    [StringLength(80)]
    public string? Ice { get; set; }
    [StringLength(80)]
    public string? CommercialRegister { get; set; }
    [StringLength(80)]
    public string? TaxRegime { get; set; }

    public bool HasInvoiceInformation =>
        !string.IsNullOrWhiteSpace(FiscalIdentifier) &&
        !string.IsNullOrWhiteSpace(ProfessionalTaxNumber) &&
        !string.IsNullOrWhiteSpace(Ice) &&
        !string.IsNullOrWhiteSpace(TaxRegime);
}
