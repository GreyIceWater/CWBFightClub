using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CWBFightClub.Services
{
    /// <summary>
    /// Class used to hash and check encrypted passwords. Taken mostly from Shevles, which came from:
    /// https://medium.com/dealeron-dev/storing-passwords-in-net-core-3de29a3da4d2
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        /// <summary>
        /// Set some constants for salt and key size.
        /// </summary>
        private const int SaltSize = 16; // 128 bit 
        private const int KeySize = 32; // 256 bit

        /// <summary>
        /// Initializes a new instance of the PasswordHasher class.
        /// </summary>
        /// <param name="options">The options injected into the class.</param>
        public PasswordHasher()
        {
            Options = new HashingOptions();
        }

        /// <summary>
        /// Gets the options for hashing.
        /// </summary>
        private HashingOptions Options { get; }

        /// <summary>
        /// Encrypt the password.
        /// </summary>
        /// <param name="password">The password to encrypt.</param>
        /// <returns>The encrypted string with iterations, salt and key hash.</returns>
        public string Hash(string password)
        {
            using (var algorithm = new Rfc2898DeriveBytes(
              password,
              SaltSize,
              Options.Iterations,
              HashAlgorithmName.SHA512))
            {
                var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
                var salt = Convert.ToBase64String(algorithm.Salt);

                return $"{Options.Iterations}.{salt}.{key}";
            }
        }

        /// <summary>
        /// Verifies a password against the stored hash.
        /// </summary>
        /// <param name="hash">The stored hash to check against.</param>
        /// <param name="password">The password being entered.</param>
        /// <returns>Returns true if it is a valid password.</returns>
        public bool Check(string hash, string password)
        {
            var parts = hash.Split('.', 3);

            if (parts.Length != 3)
            {
                throw new FormatException("Unexpected hash format. " +
                  "Should be formatted as `{iterations}.{salt}.{hash}`");
            }

            var iterations = Convert.ToInt32(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            using (var algorithm = new Rfc2898DeriveBytes(
              password,
              salt,
              iterations,
              HashAlgorithmName.SHA512))
            {
                var keyToCheck = algorithm.GetBytes(KeySize);

                bool verified = keyToCheck.SequenceEqual(key);

                return verified;
            }
        }
    }
}
