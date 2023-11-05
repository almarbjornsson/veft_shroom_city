namespace ShroomCity.Models.Exceptions;

public class UserAlreadyHasRoleException : Exception
{
    public UserAlreadyHasRoleException(string message) : base(message)
    {
    }
}