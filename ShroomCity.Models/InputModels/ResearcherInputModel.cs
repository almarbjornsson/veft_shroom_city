using System.ComponentModel.DataAnnotations;

namespace ShroomCity.Models.InputModels;


public class ResearcherInputModel
{
    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; }
}