using System.Security.Cryptography;
using System.Text;

namespace TaskManager_Simplex_.Hasher
{
    //To Hsh the password when stored in the DB
    public class PasswordHasher
    {
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        private static readonly int SaltSize = 16;
        private static readonly int HashSize = 20;
        private static readonly int Iterations = 10000;

        public static string HashPassword(string password)
        {
            byte[] salt;
            rng.GetBytes(salt = new byte[SaltSize]);
            var key = new Rfc2898DeriveBytes(password, salt, Iterations);
            var hash = key.GetBytes(HashSize);

            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            var base64Hash = Convert.ToBase64String(hashBytes);
            return base64Hash;
        }

        public static bool VerifyPassword(string password, string base64Hash)
        {
            var hashBytes = Convert.FromBase64String(base64Hash);

            var salt = new byte[SaltSize];

            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            var key = new Rfc2898DeriveBytes(password, salt, Iterations);
            byte[] hash = key.GetBytes(HashSize);

            for (var i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                    return false;
            }

            return true;
        }
    }
}

public class SecretTokenGenerator
{
    public static string GenerateSecretToken(int length = 32)
    {
        // Define characters that can be used in the token
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        // Create a random byte array to hold the token
        byte[] data = new byte[length];

        // Generate random bytes using a cryptographic random number generator
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(data);
        }

        // Convert the byte array to a string using the defined characters
        var token = new char[length];
        for (int i = 0; i < length; i++)
        {
            token[i] = chars[data[i] % chars.Length];
        }

        return new string(token);
    }

    public static void Main(string[] args)
    {
        // Generate a secret token with default length (32 characters)
        string secretToken = GenerateSecretToken();
        Console.WriteLine("Generated Secret Token: " + secretToken);
    }
}

