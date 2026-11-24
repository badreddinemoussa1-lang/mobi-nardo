using System.ComponentModel.DataAnnotations;

namespace MotoMobiNardo.Models;

public class Sale
{
    public int Id { get; set; }
    public int MotorcycleId { get; set; }
    public int CustomerId { get; set; }

    [Required, StringLength(32)]
    [Display(Name = "Canal de vente")]
    public string Channel { get; set; } = SalesChannels.Showroom;

    [Range(1, 100)]
    [Display(Name = "Quantité")]
    public int Quantity { get; set; } = 1;

    [Display(Name = "Date de vente")]
    public DateTime SoldAt { get; set; } = DateTime.Today;

    [Range(0d, 100000000d, ErrorMessage = "La remise doit être positive.")]
    [Display(Name = "Remise (DH)")]
    public decimal Discount { get; set; }

    public decimal UnitPrice { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime? CancelledAt { get; set; }
    public decimal Total => Math.Max(0, UnitPrice * Quantity - Discount);
    public string Reference => $"MMN-{Id:000000}";
    public string InvoiceNumber => $"FAC-{SoldAt.Year}-{Id:000000}";
}

public static class SalesChannels
{
    public const string Showroom = "Showroom";
    public const string Website = "Site web";
    public const string WhatsApp = "WhatsApp";
    public const string Other = "Autre";
    public const string Unspecified = "À préciser";

    public static IReadOnlyList<string> Options { get; } = [Showroom, Website, WhatsApp, Other];
}
