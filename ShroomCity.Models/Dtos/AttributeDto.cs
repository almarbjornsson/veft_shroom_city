namespace ShroomCity.Models.Dtos;

public class AttributeDto
{
    public int Id { get; set; }
    public string Value { get; set; }
    public string Type { get; set; }
    public string? RegisteredBy { get; set; }
    public DateTime? RegistrationDate { get; set; }
}