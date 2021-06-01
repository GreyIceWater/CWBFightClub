using System.ComponentModel.DataAnnotations;
using CWBFightClub.Models;
using CWBFightClub.Services;

namespace CWBFightClub.Utilities
{
    /// <summary>
    /// The class used to validated phone numbers.
    /// </summary>
    public class ValidPhoneAttribute : ValidationAttribute
    {
        /// <summary>
        /// Determines if a string is a valid phone number.
        /// </summary>
        /// <param name="value">The string to be evaluated.</param>
        /// <returns>True if valid.</returns>
        public override bool IsValid(object value)
        {
            StudentUtility studentUtil = new StudentUtility();

            string phoneNumber = (string)value;

            // Ignore null inputs.
            if (phoneNumber is null)
            {
                return true;
            }

            if (studentUtil.PhoneNumberIsValid(phoneNumber))
            {
                return true;
            }

            return false;
        }
    }
}
