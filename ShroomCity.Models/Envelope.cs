using System.Text.Json.Serialization;

namespace ShroomCity.Models;

public class Envelope<T> where T : class
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    
    [JsonPropertyName("data")]
    public IEnumerable<T> Items { get; set; } = null!;
}