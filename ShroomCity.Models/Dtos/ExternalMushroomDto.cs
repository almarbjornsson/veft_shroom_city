using System.Text.Json.Serialization;

namespace ShroomCity.Models.Dtos;

public class ExternalMushroomDto
{
    [JsonPropertyName("_id")]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string> Colors { get; set; } = new List<string>();
    public List<string> Shapes { get; set; } = new List<string>();
    public List<string> Surfaces { get; set; } = new List<string>();
}