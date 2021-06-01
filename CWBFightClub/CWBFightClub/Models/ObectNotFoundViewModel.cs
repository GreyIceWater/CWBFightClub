using System;

namespace CWBFightClub.Models
{
    /// <summary>
    /// The class used to represent an error when an object is not found.
    /// </summary>
    public class ObjectNotFoundViewModel
    {
        /// <summary>
        /// The object type that was not found.
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// Any custom messages that should be displayed to the user.
        /// </summary>
        public string CustomMessage { get; set; }
    }
}
