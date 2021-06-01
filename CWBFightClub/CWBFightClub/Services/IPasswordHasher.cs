using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWBFightClub.Services
{
    /// <summary>
    /// The interface used to define a class that will encrypt and decrypt passwords.
    /// Borrowed from Shelves
    /// </summary>
    interface IPasswordHasher
    {
        /// <summary>
        /// Encrypt the password
        /// </summary>
        /// <param name="password">The password to encrypt</param>
        /// <returns>The hash string is returned.</returns>
        string Hash(string password);

        /// <summary>
        /// Verifies a password against the stored hash.
        /// </summary>
        /// <param name="hash">The stored hash to check against.</param>
        /// <param name="password">The password being entered.</param>
        /// <returns>Returns true if it is a valid password.</returns>
        bool Check(string hash, string password);
    }
}
