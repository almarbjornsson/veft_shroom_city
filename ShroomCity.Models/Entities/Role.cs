namespace ShroomCity.Models.Entities;


public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
