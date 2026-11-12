using System.Security.Cryptography;

namespace MagasinGestion.Security;

public sealed class PasswordHasher
{
    public const int Iterations = 210_000;
    public (byte[] Hash, byte[] Salt, int Iterations) Hash(string password)
    { var salt = RandomNumberGenerator.GetBytes(16); return (Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32), salt, Iterations); }
    public bool Verify(string password, byte[] hash, byte[] salt, int iterations) => CryptographicOperations.FixedTimeEquals(Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, hash.Length), hash);
}
