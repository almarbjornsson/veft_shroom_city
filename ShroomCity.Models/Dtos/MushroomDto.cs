using System.Text.Json.Serialization;

namespace ShroomCity.Models.Dtos;

public class MushroomDto
{
    // Ignore this property when serializing to JSON if it's null
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
}