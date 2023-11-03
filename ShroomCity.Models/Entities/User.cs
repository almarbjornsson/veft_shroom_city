namespace ShroomCity.Models.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Bio { get; set; }
    public string HashedPassword { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string EmailAddress { get; set; }
    public ICollection<Role> Roles { get; set; } = new List<Role>();
    public ICollection<Attribute> RegisteredAttributes { get; set; } = new List<Attribute>();
}