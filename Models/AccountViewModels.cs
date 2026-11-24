using System.ComponentModel.DataAnnotations;

namespace MotoMobiNardo.Models;

public class LoginViewModel
{
    [Required, EmailAddress]
    [Display(Name = "Adresse e-mail")]
    public string Email { get; set; } = "";

    [Required, DataType(DataType.Password)]
    [Display(Name = "Mot de passe")]
    public string Password { get; set; } = "";

    [Display(Name = "Garder ma session ouverte")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

public class SetupViewModel
{
    public string? BootstrapKey { get; set; }

    [Required, EmailAddress]
    [Display(Name = "Adresse e-mail administrateur")]
    public string Email { get; set; } = "";

    [Required, DataType(DataType.Password), MinLength(12)]
    [Display(Name = "Mot de passe")]
    public string Password { get; set; } = "";

    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    [Display(Name = "Confirmer le mot de passe")]
    public string ConfirmPassword { get; set; } = "";
}
