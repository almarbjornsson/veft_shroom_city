using System.ComponentModel.DataAnnotations;

namespace ShroomCity.Models.InputModels;

public class RegisterInputModel
{
    [Required]
    [MinLength(5)]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    public string PasswordConfirmation { get; set; }
}