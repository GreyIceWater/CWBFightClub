using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWBFightClub.Services
{
    /// <summary>
    /// The class used to hold the hashing options.
    /// Borrowed from Shelves
    /// </summary>
    public class HashingOptions
    {
        /// <summary>
        /// Initializes a new instance of the HashingOptions class.
        /// </summary>
        public HashingOptions()
        {
            this.Iterations = 1000;
        }

        /// <summary>
        /// Gets or sets the number of times to run the hashing algorithm.
        /// </summary>
        public int Iterations { get; set; }
    }
}
