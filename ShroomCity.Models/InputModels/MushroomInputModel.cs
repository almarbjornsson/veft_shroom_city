using System.ComponentModel.DataAnnotations;

namespace ShroomCity.Models.InputModels;


public class MushroomInputModel
{
    [Required]
    public string Name { get; set; }

    public string? Description { get; set; }
}
