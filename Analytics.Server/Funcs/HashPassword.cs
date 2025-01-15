using System.Security.Cryptography;

namespace Analytics.Server
{
    public class HashPassword
    {
        private static byte[] GenerateSalt(int size = 16)
        {
            byte[] salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private static string GetHashPassword(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32); // Длина хеша 32 байта (256 бит)
                return Convert.ToBase64String(hash);
            }
        }

        public static (string, byte[]) GetHashPassword(string password)
        {
            var salt = GenerateSalt();

            return (GetHashPassword(password, salt), salt);
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash, byte[] storedSalt)
        {
            string hashOfEnteredPassword = GetHashPassword(enteredPassword, storedSalt);
            return hashOfEnteredPassword == storedHash;
        }
    }
}