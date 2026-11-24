using System.ComponentModel.DataAnnotations;

namespace MotoMobiNardo.Models;

public class Customer
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Nom complet")]
    public string FullName { get; set; } = "";

    [Required, Phone]
    [Display(Name = "Téléphone")]
    public string Phone { get; set; } = "";

    [EmailAddress]
    [Display(Name = "E-mail")]
    public string? Email { get; set; }

    [StringLength(160)]
    [Display(Name = "Adresse")]
    public string? Address { get; set; }
}
