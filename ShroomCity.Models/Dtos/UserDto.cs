namespace ShroomCity.Models.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string EmailAddress { get; set; }
    public string? Bio { get; set; }
    public List<string> Permissions { get; set; } = new List<string>();
    public int TokenId { get; set; }
}