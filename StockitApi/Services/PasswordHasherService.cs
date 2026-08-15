using System.Security.Cryptography;
using System.Text;
using StockitApi.Interfaces;

namespace StockitApi.Services;

public class PasswordHasherService : IPasswordHasherService
{
    private const int Iterations = 100_000;
    private const int HashByteSize = 32; // bytes - 256 bits
    private const int SaltByteSize = 16; // bytes - 128 bits

    public (string hashedPassword, byte[] salt) HashPassword(string password)
    {
        byte[] salt = GenerateSalt();
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashByteSize);
        return (Convert.ToBase64String(hash), salt);
    }

    private static byte[] GenerateSalt()
    {
        byte[] salt = new byte[SaltByteSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword, byte[] salt)
    {
        var hashed = SHA256.HashData(Encoding.UTF8.GetBytes(providedPassword + Convert.ToBase64String(salt)));
        return Convert.ToBase64String(hashed) == hashedPassword;
    }
}