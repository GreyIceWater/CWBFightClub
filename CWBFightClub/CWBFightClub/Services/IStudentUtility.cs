using CWBFightClub.Data;
using CWBFightClub.Models;
using System.Collections.Generic;

namespace CWBFightClub.Services
{
    /// <summary>
    /// The interface used for student business logic.
    /// </summary>
    public interface IStudentUtility
    {
        /// <summary>
        /// Converts a string to a format usable by a mobile device.
        /// </summary>
        /// <param name="unformattedNumber">The unformatted string to format.</param>
        /// <returns>The mobile format for the number is returned.</returns>
        string ConvertNumberToMobileFormat(string unformattedNumber);

        /// <summary>
        /// Determines if a string is a valid number.
        /// </summary>
        /// <param name="phoneNumberToCheck">The string to check.</param>
        /// <returns>True if valid.</returns>
        bool PhoneNumberIsValid(string phoneNumberToCheck);

        void UpdateStudentBalance(Student student, CWBContext db);
        void UpdateStudentBalance(IEnumerable<Student> students, CWBContext db);
        void VerifyAttendanceRecord(IEnumerable<Student> students, CWBContext db);
    }
}
