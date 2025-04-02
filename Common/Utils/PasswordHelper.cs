using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace npascu_api_v1.Common.Utils
{
    public static class PasswordHelper
    {
        /// <summary>
        /// Hashes a password using Argon2id.
        /// The output format is: {iterations}.{memorySize}.{parallelism}.{salt}.{hash}
        /// </summary>
        public static string HashPassword(string password)
        {
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                Iterations = 4,
                MemorySize = 64 * 1024, // 64 MB
                DegreeOfParallelism = 4
            };

            var hash = argon2.GetBytes(32);

            return $"{argon2.Iterations}.{argon2.MemorySize}.{argon2.DegreeOfParallelism}." +
                   $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        /// <summary>
        /// Verifies a password against a stored Argon2id hash.
        /// </summary>
        public static bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split('.');
            if (parts.Length != 5)
            {
                throw new FormatException(
                    "Unexpected hash format. Should be 'iterations.memorySize.parallelism.salt.hash'");
            }

            var iterations = int.Parse(parts[0]);
            var memorySize = int.Parse(parts[1]);
            var degreeOfParallelism = int.Parse(parts[2]);
            var salt = Convert.FromBase64String(parts[3]);
            var expectedHash = Convert.FromBase64String(parts[4]);

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                Iterations = iterations,
                MemorySize = memorySize,
                DegreeOfParallelism = degreeOfParallelism
            };

            var actualHash = argon2.GetBytes(expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
}