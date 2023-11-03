namespace ShroomCity.Models.Entities;

public class AttributeType
{
    public int Id { get; set; }
    public string Type { get; set; }
    public ICollection<Attribute> Attributes { get; set; } = new List<Attribute>();
}