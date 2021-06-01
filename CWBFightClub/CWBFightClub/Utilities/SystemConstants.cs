namespace CWBFightClub.Utilities
{
    /// <summary>
    /// Class used to contain system constants.
    /// </summary>
    public class SystemConstants
    {
        /// <summary>
        /// Minimum student age.
        /// </summary>
        public const int MinAge = 10;

        /// <summary>
        /// Maximum student age.
        /// </summary>
        public const int MaxAge = 100;

        /// <summary>
        /// Two letter country code used for phone numbers.
        /// </summary>
        public const string PhoneCountryCode = "US";

        /// <summary>
        /// The root path used for saving database backups.
        /// </summary>
        public const string RootPath = @"C:\DatabaseBackups";

        /// <summary>
        /// The sub-directory path to save the actual .bak files.
        /// </summary>
        public const string SubdirectoryPath = @"C:\DatabaseBackups\CWBFightClub";

        /// <summary>
        /// Name of the database.
        /// </summary>
        public const string DatabaseName = "CWBFightClub";

        /// <summary>
        /// Name used for Walk in discipline and scheduled class.
        /// </summary>
        public const string Walkin = "Walk In";

        /// <summary>
        /// Minutes that will allow the enrolled students to check in early.
        /// </summary>
        public const int MinutesAllowedForCheckinEarly = 30;

        /// <summary>
        /// Number of items on a page. 
        /// </summary>
        public static int ItemsPerPage = 10;
    }
}
