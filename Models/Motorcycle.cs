using System.ComponentModel.DataAnnotations;

namespace MotoMobiNardo.Models;

public class Motorcycle
{
    public int Id { get; set; }

    [Required, StringLength(60)]
    [Display(Name = "Marque")]
    public string Brand { get; set; } = "";

    [Required, StringLength(80)]
    [Display(Name = "Modèle")]
    public string Model { get; set; } = "";

    [Range(1950, 2100)]
    [Display(Name = "Année")]
    public int Year { get; set; } = DateTime.Today.Year;

    [Range(1, 2500)]
    [Display(Name = "Cylindrée (cc)")]
    public int EngineCc { get; set; } = 125;

    [Range(0.01d, 100000000d)]
    [Display(Name = "Prix (DH)")]
    public decimal Price { get; set; }

    [Range(0, 10000)]
    [Display(Name = "Stock")]
    public int Stock { get; set; }

    [StringLength(500)]
    [Display(Name = "Description")]
    public string Description { get; set; } = "";

    [StringLength(100)]
    [Display(Name = "Photo enregistrée")]
    public string? PhotoFileName { get; set; }

    [Required, StringLength(24)]
    [Display(Name = "Catégorie")]
    public string Category { get; set; } = MotorcycleCategories.Motorcycle;

    [Required, StringLength(24)]
    [Display(Name = "État")]
    public string Condition { get; set; } = MotorcycleConditions.Used;

    public string FullName => $"{Brand} {Model}";
}

public static class MotorcycleCategories
{
    public const string Motorcycle = "Moto";
    public const string Scooter = "Scooter";
    public static IReadOnlyList<string> Options { get; } = [Motorcycle, Scooter];
}

public static class MotorcycleConditions
{
    public const string New = "Neuve";
    public const string Used = "Occasion";
    public static IReadOnlyList<string> Options { get; } = [New, Used];
}
