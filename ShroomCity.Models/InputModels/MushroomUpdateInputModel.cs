using System.ComponentModel.DataAnnotations;

namespace ShroomCity.Models.InputModels;

public class MushroomUpdateInputModel
{
    // [Required] ?
    public string Name { get; set; }

    public string? Description { get; set; }
}