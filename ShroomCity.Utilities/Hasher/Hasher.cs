using System.Security.Cryptography;

namespace ShroomCity.Utilities.Hasher;

public static class Hasher
{
    public static string HashPassword(string password, string salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), 10000, HashAlgorithmName.SHA256);
        byte[] hashBytes = pbkdf2.GetBytes(32); // 32 bytes = 256 bits
        string hashedPassword = Convert.ToBase64String(hashBytes);

        // Combine the salt and hash and convert to a Base64 string
        string combined = $"{salt}.{hashedPassword}";
        return combined;
    }
}