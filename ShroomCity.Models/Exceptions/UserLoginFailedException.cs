namespace ShroomCity.Models.Exceptions;

public class UserLoginFailedException : Exception
{
    public UserLoginFailedException(string message) : base(message)
    {
    }
}