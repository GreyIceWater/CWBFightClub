using CWBFightClub.Data;
using CWBFightClub.Models;
using CWBFightClub.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWBFightClub.Controllers
{
    public class AchievedBeltController : BaseController
    {
        private static IEnrollmentUtility _enrollUtility;
        public AchievedBeltController(CWBContext db, IAccessChecker ac, IEnrollmentUtility enrollmentUtility) : base(ac, db)
        {
            _enrollUtility = enrollmentUtility;
        }

        /// <summary>
        /// Add the next rank of belt to the achieved belts for the enrollment.
        /// </summary>
        /// <param name="studentID">The ID of the student to add the achieved belt to.</param>
        /// <param name="enrollmentID">The ID of the enrollment to modify.</param>
        /// <returns>Redirects to Student Enrollment</returns>
        public async Task<IActionResult> Add(int studentID, int enrollmentID)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            Belt nextBelt = await _enrollUtility.NextBeltIs(enrollmentID);

            if (nextBelt is not null)
            {
                AchievedBelt achievedBelt = new AchievedBelt
                {
                    EnrollmentID = enrollmentID,
                    Name = nextBelt.Name,
                    Description = nextBelt.BeltDescription,
                    Rank = nextBelt.Rank,
                    DateAchieved = DateTime.Now
                };

                AssignCreator(achievedBelt);
                _db.AchievedBelts.Add(achievedBelt);
                _db.SaveChanges();
            }

            bool fromStudentWorkflow = TempData["FromStudentWorkflow"] != null ? (bool)TempData["FromStudentWorkflow"] : false;

            return RedirectToAction("Student", "Enrollment", new { id = studentID, enrollmentID = enrollmentID, fromStudentWorkflow });
        }

        /// <summary>
        /// Updates the achieved belt date achieved field.
        /// </summary>
        /// <param name="id">The ID of the achieved belt.</param>
        /// <param name="newDate">The new date achieved.</param>
        /// <param name="studentID">The ID of the student associated with this achieved belt.</param>
        /// <param name="enrollmentID">The ID of the enrollment associated with the achieved belt.</param>
        /// <returns>Redirects to student enrollment.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(int id, DateTime newDate, int studentID, int enrollmentID)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            AchievedBelt belt = await _db.AchievedBelts.FindAsync(id);

            if (belt is null || belt.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", new { type = typeof(Enrollment).Name, message = "Edit failed." });
            }

            AssignModifier(belt);
            belt.DateAchieved = newDate;

            _db.AchievedBelts.Update(belt);
            _db.SaveChanges();

            bool fromStudentWorkflow = TempData["FromStudentWorkflow"] != null ? (bool)TempData["FromStudentWorkflow"] : false;

            return RedirectToAction("Student", "Enrollment", new { id = studentID, enrollmentID =  enrollmentID, fromStudentWorkflow });
        }

        /// <summary>
        /// Deletes an achieved belt for a student enrollment.
        /// </summary>
        /// <param name="id">The ID of the achieved belt to delete.</param>
        /// <param name="studentID">The ID of the student.</param>
        /// <param name="enrollmentID">The ID of the enrollment.</param>
        /// <returns>Redirects to student enrollments.</returns>
        public async Task<IActionResult> Delete(int id, int studentID, int enrollmentID)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            AchievedBelt belt = await _db.AchievedBelts.FindAsync(id);

            if (belt is null || belt.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", new { type = typeof(Enrollment).Name, message = "Edit failed." });
            }

            AssignModifier(belt);
            belt.IsArchived = true;

            _db.AchievedBelts.Update(belt);
            _db.SaveChanges();

            bool fromStudentWorkflow = TempData["FromStudentWorkflow"] != null ? (bool)TempData["FromStudentWorkflow"] : false;
            return RedirectToAction("Student", "Enrollment", new { id = studentID, enrollmentID = enrollmentID, fromStudentWorkflow });
        }
    }
}
