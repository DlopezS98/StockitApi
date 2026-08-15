namespace StockitApi.Interfaces;

public interface IPasswordHasherService
{
    (string hashedPassword, byte[] salt) HashPassword(string password);
    bool VerifyHashedPassword(string hashedPassword, string providedPassword, byte[] salt);
}
