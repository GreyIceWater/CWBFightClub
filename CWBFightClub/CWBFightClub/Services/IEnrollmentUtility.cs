using CWBFightClub.Models;
using System.Threading.Tasks;

namespace CWBFightClub.Services
{
    /// <summary>
    /// The interface used for student business logic.
    /// </summary>
    public interface IEnrollmentUtility
    {
        Task<bool> NextBeltExists(int enrollmentID);
        Task<Belt> NextBeltIs(int enrollmentID);
    }
}
