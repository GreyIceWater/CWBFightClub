using CWBFightClub.Data;
using CWBFightClub.Models;
using CWBFightClub.Services;
using CWBFightClub.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWBFightClub.Controllers
{
    public class GuardianController : BaseController
    {
        private readonly IStudentUtility _studentUtil;

        public GuardianController(CWBContext db, IStudentUtility studentUtil, IAccessChecker ac) : base(ac, db)
        {
            _studentUtil = studentUtil;
        }

        // Generates view to create a guardian for a student. id = student ID.
        public IActionResult Create(int id, bool fromStudentWorkflow = false)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                ViewBag.NoLogin = true;
            }
            else
            {
                ViewBag.NoLogin = false;
            }

            TempData["FromStudentWorkflow"] = fromStudentWorkflow;

            Student student = FindStudent(id);

            if (student != null)
            {
                ViewBag.StudentName = student.FirstName;
                ViewBag.studentid = student.StudentID;
            }
            else
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Student).Name, message = "No student found. Please ensure you are adding a guardian from a student." });
            }

            return View();
        }

        // Post method for create.
        [HttpPost]
        public async Task<IActionResult> Create(Guardian guardian, int studentid, bool fromStudentWorkflow = false)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                ViewBag.NoLogin = true;
            }
            else
            {
                ViewBag.NoLogin = false;
            }

            if (guardian is not null)
            {
                if (!string.IsNullOrWhiteSpace(guardian.Phone))
                {
                    guardian.Phone = _studentUtil.ConvertNumberToMobileFormat(guardian.Phone);
                }

                if (!ModelState.IsValid)
                {
                    return View(guardian);
                }

                AssignCreator(guardian);

                _db.Guardians.Add(guardian);
                await _db.SaveChangesAsync();
                int id = guardian.GuardianID;

                StudentGuardian studentguardian = new StudentGuardian();

                studentguardian.GuardianID = id;
                studentguardian.StudentID = studentid;
                studentguardian.IsPrimary = guardian.IsPrimary;
                studentguardian.Relationship = guardian.Relationship;
                AssignCreator(studentguardian);
                _db.StudentGuardians.Add(studentguardian);
                await _db.SaveChangesAsync();

                ThereCanBeOnlyOne(studentguardian);
            }

            return RedirectToAction("Student", "Guardian", new { studentid = studentid, fromStudentWorkflow });
        }

        // Link an existing guardian to a student.
        public IActionResult Link(int guardianid, int studentid, bool fromStudentWorkflow = false)
        {
            // No security check here because we need to allow this to happen for non-logged in users.
            TempData["FromStudentWorkflow"] = fromStudentWorkflow;

            if (guardianid != 0 && studentid != 0)
            {
                StudentGuardian studentGuardian = new StudentGuardian()
                {
                    StudentID = studentid,
                    GuardianID = guardianid
                };

                AssignCreator(studentGuardian);

                _db.StudentGuardians.Add(studentGuardian);
                _db.SaveChanges();
                //fromStudentWorkflow = TempData["FromStudentWorkflow"] != null ? (bool)TempData["FromStudentWorkflow"] : false;
                return RedirectToAction("Student", "Guardian", new { studentid = studentid, fromStudentWorkflow });
            }

            return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(StudentGuardian).Name, message = "Link failed." });
        }

        /// <summary>
        /// Archives and subsequently unlinks a StudentGuardian record from a student.
        /// </summary>
        /// <param name="guardianid">Guardian to remove from student.</param>
        /// <param name="studentid">Student with the Guardian to remove.</param>
        /// <returns>The index view for Guardians.</returns>
        public async Task<IActionResult> Unlink(int studentguardianid, int studentid)
        {
            // No security check so the workflow is consistent for logged in and non-logged in users.

            if (studentguardianid != 0 && studentid != 0)
            {
                try
                {
                    var studentGuardianToArchive = await _db.FindAsync<StudentGuardian>(studentguardianid);

                    if (studentGuardianToArchive == null || studentGuardianToArchive.IsArchived == true)
                    {
                        return RedirectToAction("ObjectNotFound", "base", new { type = typeof(Guardian).Name, message = "Delete failed. No available guardian." });
                    }

                    studentGuardianToArchive.IsArchived = true;

                    AssignModifier(studentGuardianToArchive);
                    _db.StudentGuardians.Update(studentGuardianToArchive);
                    await _db.SaveChangesAsync();

                    // Checks for active student guardian records. If there are none, the guardian will also be archived.
                    await CheckForStudentGuardianRecords(studentGuardianToArchive.GuardianID);
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw new DbUpdateConcurrencyException("Could not update database.");
                }
                catch (Exception)
                {
                    throw new Exception("Error deleting guardian from student.");
                }
            }
            else
            {
                return RedirectToAction("ObjectNotFound", "base", new { type = "Error. No associated student or guardian.", message = "Delete failed" });
            }

            bool fromStudentWorkflow = TempData["FromStudentWorkflow"] != null ? (bool)TempData["FromStudentWorkflow"] : false;

            return RedirectToAction("Student", new { studentid = studentid, fromStudentWorkflow });
        }

        // Depricated: Delete a guardian. Use unlink instead and if no students are attached to the guardian it will auto archive the guardian.
        public async Task<IActionResult> Delete(int? id, int studentid, int studentguardianid)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (id is null)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Guardian).Name, message = "Deletion failed." });
            }

            Guardian guardian = await _db.FindAsync<Guardian>(id);

            if (guardian is null || guardian.IsArchived is true)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Guardian).Name, message = "Deletion failed." });
            }

            ViewBag.studentid = studentid;
            ViewBag.studentguardianid = studentguardianid;
            TempData["FromStudentWorkflow"] = TempData["FromStudentWorkflow"];
            return View(guardian);
        }

        // Depricated: Delete a guardian. Use unlink instead and if no students are attached to the guardian it will auto archive the guardian.
        [HttpPost]
        public async Task<IActionResult> DeletePost(int? id, int studentid)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (id is null)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Guardian).Name, message = "Deletion failed." });
            }

            Guardian guardian = await _db.FindAsync<Guardian>(id);
            ViewBag.studentid = studentid;

            if (guardian is null || guardian.IsArchived is true)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Guardian).Name, message = "Deletion failed." });
            }

            AssignModifier(guardian);
            guardian.IsArchived = true;

            _db.Guardians.Update(guardian);
            _db.SaveChanges();

            guardian = await _db.Guardians.Include(x => x.StudentGuardians).Where(x => x.GuardianID == id).FirstOrDefaultAsync();
            foreach (StudentGuardian sg in guardian.StudentGuardians)
            {
                sg.IsArchived = true;
                _db.StudentGuardians.Update(sg);
            }
            _db.SaveChanges();
            bool fromStudentWorkflow = TempData["FromStudentWorkflow"] != null ? (bool)TempData["FromStudentWorkflow"] : false;
            return RedirectToAction("Student", "Guardian", new { studentid = studentid });
        }

        // Generate Edit a guardian form. id = guardian ID.
        public async Task<IActionResult> Edit(int? id, int studentid, int studentguardianid)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            ViewBag.studentid = studentid;
            ViewBag.studentguardianid = studentguardianid;

            if (id is null)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Guardian).Name, message = "Edit failed." });
            }

            Guardian guardian = await _db.FindAsync<Guardian>(id);
            StudentGuardian sg = await _db.FindAsync<StudentGuardian>(studentguardianid);
            guardian.IsPrimary = sg.IsPrimary;
            guardian.Relationship = sg.Relationship;

            if (guardian is null || guardian.IsArchived is true)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Guardian).Name, message = "Edit failed." });
            }

            TempData["FromStudentWorkflow"] = TempData["FromStudentWorkflow"];
            return View(guardian);
        }

        // Post method for edit.
        // Calls ThereCanBeOnlyOne to ensure only one active guardian is set to primary for each student.
        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditPost(Guardian guardian, int studentid, int studentguardianid)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (guardian == null)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Guardian).Name, message = "Edit failed." });
            }

            var guardianToUpdate = await _db.FindAsync<Guardian>(guardian.GuardianID);
            ViewBag.studentid = studentid;
            ViewBag.studentguardianid = studentguardianid;

            if (guardianToUpdate is null || guardianToUpdate.IsArchived is true)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Guardian).Name, message = "Edit failed." });
            }

            if (!ModelState.IsValid)
            {
                return View(guardianToUpdate);
            }

            if (await TryUpdateModelAsync<Guardian>(guardianToUpdate, "",
                x => x.FirstName,
                x => x.MiddleName,
                x => x.LastName,
                x => x.StreetAddress,
                x => x.City,
                x => x.State,
                x => x.ZIP,
                x => x.Phone,
                x => x.Email))
            {

                if (!string.IsNullOrWhiteSpace(guardianToUpdate.Phone))
                {
                    guardianToUpdate.Phone = _studentUtil.ConvertNumberToMobileFormat(guardianToUpdate.Phone);
                }

                AssignModifier(guardianToUpdate);

                _db.Guardians.Update(guardianToUpdate);

                StudentGuardian sg = await _db.StudentGuardians.FindAsync(studentguardianid);
                sg.IsPrimary = guardian.IsPrimary;
                sg.Relationship = guardian.Relationship;
                _db.StudentGuardians.Update(sg);

                _db.SaveChanges();

                ViewBag.studentid = studentid;
                bool fromStudentWorkflow = TempData["FromStudentWorkflow"] != null ? (bool)TempData["FromStudentWorkflow"] : false;

                ThereCanBeOnlyOne(sg);

                return RedirectToAction("Student", "Guardian", new { studentid = studentid, fromStudentWorkflow });
            }

            return View(guardianToUpdate);
        }

        // List of guardians for a student.
        public async Task<IActionResult> Student(int studentid, string sortOrder, string searchString, int? pageNumber = 1, bool fromStudentWorkflow = false)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            ViewBag.noLogin = checkResult == null ? false : true;
            ViewBag.FromStudentWorkflow = fromStudentWorkflow;
            TempData["FromStudentWorkflow"] = fromStudentWorkflow;

            ViewBag.studentid = studentid;
            Student student = FindStudent(studentid);

            ViewBag.studentname = student.FirstName + " " + student.LastName;

            IEnumerable<StudentGuardian> studentguardians = null;

            await Task.Run(() =>
            {
                studentguardians = _db.StudentGuardians.Where(x => x.IsArchived == false && x.StudentID == studentid).Include(x => x.Guardian)
                               .AsNoTracking();
            });

            List<int> guardianids = studentguardians.Select(x => x.GuardianID).ToList();

            List<Guardian> guardians = _db.Guardians.Where(x => x.IsArchived == false && !guardianids.Contains(x.GuardianID)).ToList();

            var selectlist = new List<SelectListItem>();

            foreach (Guardian guardian in guardians)
            {
                selectlist.Add(new SelectListItem
                {
                    Text = guardian.FirstName + " " + guardian.LastName,
                    Value = guardian.GuardianID.ToString()
                });
            }

            ViewBag.availableguardians = selectlist;
            ViewBag.guardianscount = selectlist.Count;

            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentSearch"] = searchString;

            ViewData["FirstNameSortParm"] = sortOrder == "FirstName" ? "firstname_desc" : "FirstName";
            ViewData["LastNameSortParm"] = sortOrder == "LastName" ? "lastname_desc" : "LastName";
            ViewData["PhoneSortParm"] = sortOrder == "Phone" ? "phone_desc" : "Phone";

            switch (sortOrder)
            {
                case "FirstName":
                    studentguardians = studentguardians.OrderBy(sg => sg.Guardian.FirstName);
                    break;
                case "firstname_desc":
                    studentguardians = studentguardians.OrderByDescending(sg => sg.Guardian.FirstName);
                    break;
                case "LastName":
                    studentguardians = studentguardians.OrderBy(sg => sg.Guardian.LastName);
                    break;
                case "lastname_desc":
                    studentguardians = studentguardians.OrderByDescending(sg => sg.Guardian.LastName);
                    break;
                case "Phone":
                    studentguardians = studentguardians.OrderBy(sg => sg.Guardian.Phone);
                    break;
                case "phone_desc":
                    studentguardians = studentguardians.OrderByDescending(sg => sg.Guardian.Phone);
                    break;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                studentguardians = studentguardians.Where(sg => sg.Guardian.FirstName.ToLower().Contains(searchString.ToLower()) ||
                            sg.Guardian.LastName.ToLower().Contains(searchString.ToLower()));
            }

            return View(PaginatedList<StudentGuardian>.Create(studentguardians.AsQueryable(), pageNumber.Value, SystemConstants.ItemsPerPage));
        }

        /// <summary>
        /// Gathers a list of guardians available to add to a student that the student does not already have.
        /// </summary>
        /// <param name="studentid">Student to add to.</param>
        /// <param name="sortOrder">Sort order for the guardians, defaulted to null.</param>
        /// <param name="searchString">Search string to use on the guardians.</param>
        /// <param name="pageNumber">Current page number.</param>
        /// <returns>Returns a list of guardians not already added to the student.</returns>
        public async Task<IActionResult> AllGuardians(int studentid, bool fromStudentWorkflow, int fromWorkflow = 2, string sortOrder = null, string searchString = null, int? pageNumber = 1)
        {
            ViewBag.studentid = studentid;
            Student student = FindStudent(studentid);

            ViewBag.studentname = student.FirstName + " " + student.LastName;
            if (fromWorkflow == 2)
            {
                ViewBag.FromStudentWorkflow = fromStudentWorkflow;
            }
            else
            {
                ViewBag.FromStudentWorkflow = fromWorkflow == 1 ? true : false;
            }
            

            IEnumerable<StudentGuardian> studentGuardians = null;

            try
            {
                await Task.Run(() =>
                {
                    studentGuardians = _db.StudentGuardians.Where(x => !x.IsArchived && x.StudentID == studentid).Include(x => x.Guardian)
                                   .AsNoTracking();
                });

                IEnumerable<int> guardianids = studentGuardians.Select(x => x.GuardianID).ToList();
                IEnumerable<Guardian> guardians = await _db.Guardians.Where(x => !x.IsArchived && !guardianids.Contains(x.GuardianID)).ToListAsync();

                ViewData["CurrentSort"] = sortOrder;
                ViewData["CurrentSearch"] = searchString;

                ViewData["FirstNameSortParm"] = sortOrder == "FirstName" ? "firstname_desc" : "FirstName";
                ViewData["LastNameSortParm"] = sortOrder == "LastName" ? "lastname_desc" : "LastName";
                ViewData["StreetSortParm"] = sortOrder == "StreetAddress" ? "streetaddress_desc" : "StreetAddress";
                ViewData["PhoneSortParm"] = sortOrder == "Phone" ? "phone_desc" : "Phone";
                ViewData["EmailSortParm"] = sortOrder == "Email" ? "email_desc" : "Email";

                switch (sortOrder)
                {
                    case "FirstName":
                        guardians = guardians.OrderBy(x => x.FirstName);
                        break;
                    case "firstname_desc":
                        guardians = guardians.OrderByDescending(x => x.FirstName);
                        break;
                    case "LastName":
                        guardians = guardians.OrderBy(x => x.LastName);
                        break;
                    case "lastname_desc":
                        guardians = guardians.OrderByDescending(x => x.LastName);
                        break;
                    case "StreetAddress":
                        guardians = guardians.OrderBy(x => x.StreetAddress);
                        break;
                    case "streetaddress_desc":
                        guardians = guardians.OrderByDescending(x => x.Email);
                        break;
                    case "Phone":
                        guardians = guardians.OrderBy(x => x.Phone);
                        break;
                    case "phone_desc":
                        guardians = guardians.OrderByDescending(x => x.Phone);
                        break;
                    case "Email":
                        guardians = guardians.OrderBy(x => x.Email);
                        break;
                    case "email_desc":
                        guardians = guardians.OrderByDescending(x => x.Email);
                        break;
                }

                if (!String.IsNullOrEmpty(searchString))
                {
                    guardians = guardians.Where(x => x.FirstName.ToLower().Contains(searchString.ToLower()) ||
                                x.LastName.ToLower().Contains(searchString.ToLower()));
                }

                return View(PaginatedList<Guardian>.Create(guardians.AsQueryable(), pageNumber.Value, SystemConstants.ItemsPerPage));
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Locates the student by passed in ID.
        /// </summary>
        /// <param name="id">The studentID to search on</param>
        /// <returns>The located student.</returns>
        private Student FindStudent(int id)
        {
            var student = _db.Students.Where(x => x.StudentID == id && x.IsArchived == false).FirstOrDefault();

            return student;
        }

        /// <summary>
        /// If the passed in student guardian is set to primary, find all other active student guardian records and
        /// set IsPrimary to false.
        /// </summary>
        /// <param name="sg">The current student guardian record that was just created or editted.</param>
        private void ThereCanBeOnlyOne(StudentGuardian sg)
        {
            if (sg.IsPrimary == true)
            {
                IEnumerable<StudentGuardian> studentGuardians = _db.StudentGuardians
                    .Where(x => x.IsArchived == false && x.StudentID == sg.StudentID && x.StudentGuardianID != sg.StudentGuardianID);

                foreach (StudentGuardian s in studentGuardians)
                {
                    s.IsPrimary = false;
                    _db.StudentGuardians.Update(s);
                }

                _db.SaveChanges();
            }
        }
    }
}