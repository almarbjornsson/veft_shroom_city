using System.Text.Json.Serialization;

namespace ShroomCity.Models.Dtos;

public class ExternalAttributeDto
{
    [JsonPropertyName("_id")]
    public string Id { get; set; }
    public string Name { get; set; }
}