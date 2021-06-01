using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CWBFightClub.Data;
using CWBFightClub.Models;
using CWBFightClub.Utilities;
using Microsoft.EntityFrameworkCore;
using PhoneNumbers;

namespace CWBFightClub.Services
{
    /// <summary>
    /// The class used for business logic for the enrollment object.
    /// </summary>
    public class EnrollmentUtility : IEnrollmentUtility
    {
        private static CWBContext _db;
        public EnrollmentUtility(CWBContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Determines if next belt exists for achieved belt purposes.
        /// </summary>
        /// <param name="enrollmentID"></param>
        /// <returns></returns>
        public async Task<bool> NextBeltExists(int enrollmentID)
        {
            Belt exists = await NextBeltIs(enrollmentID);

            if (exists is null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Retrieves the next belt, if it exists, for a particular enrollment.
        /// </summary>
        /// <param name="enrollmentID">The enrollment ID to check for next belt.</param>
        /// <returns>Returns the belt next in line per rank.</returns>
        public async Task<Belt> NextBeltIs(int enrollmentID)
        {
            int currentRank = await _db.AchievedBelts
                .Where(x => !x.IsArchived && x.EnrollmentID == enrollmentID)
                .OrderByDescending(x => x.Rank)
                .Select(x => x.Rank)
                .FirstOrDefaultAsync();

            int disciplineID = await _db.Enrollments
                .Where(x => x.EnrollmentID == enrollmentID && !x.IsArchived)
                .Select(x => x.DisciplineID)
                .FirstOrDefaultAsync();

            List<Belt> discBelts = await _db.Belts.Where(x => x.IsArchived == false && x.DisciplineID == disciplineID).OrderBy(x => x.Rank).ToListAsync();
            Belt nextBelt = discBelts.Where(x => x.Rank > currentRank).FirstOrDefault();

            return nextBelt;
        }
    }
}
