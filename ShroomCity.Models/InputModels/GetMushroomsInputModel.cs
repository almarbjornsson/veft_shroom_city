using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShroomCity.Models.InputModels;

public class GetMushroomsInputModel : IValidatableObject
{
    public string? Name { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Stem size minimum must be greater than or equal to 0.")]
    public int? StemSizeMinimum { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Stem size maximum must be greater than or equal to 0.")]
    public int? StemSizeMaximum { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Cap size minimum must be greater than or equal to 0.")]
    public int? CapSizeMinimum { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Cap size maximum must be greater than or equal to 0.")]
    public int? CapSizeMaximum { get; set; }
    
    public string? Color { get; set; }
    
    [DefaultValue(25)]
    [Range(1, int.MaxValue, ErrorMessage = "Page size must be greater than 0.")]
    public int PageSize { get; set; } = 25;
    
    [DefaultValue(1)]
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0.")]
    public int PageNumber { get; set; } = 1;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StemSizeMinimum.HasValue && StemSizeMaximum.HasValue)
        {
            if (StemSizeMinimum.Value > StemSizeMaximum.Value)
            {
                yield return new ValidationResult(
                    "Stem size maximum must be greater than or equal to stem size minimum.",
                    new List<string> { nameof(StemSizeMinimum), nameof(StemSizeMaximum) });
            }
        }

        if (CapSizeMinimum.HasValue && CapSizeMaximum.HasValue)
        {
            if (CapSizeMinimum.Value > CapSizeMaximum.Value)
            {
                yield return new ValidationResult(
                    "Cap size maximum must be greater than or equal to cap size minimum.",
                    new List<string> { nameof(CapSizeMinimum), nameof(CapSizeMaximum) });
            }
        }
    }
}
