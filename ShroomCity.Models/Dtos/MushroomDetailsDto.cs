namespace ShroomCity.Models.Dtos;

public class MushroomDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public List<AttributeDto> Attributes { get; set; } = new List<AttributeDto>();
}