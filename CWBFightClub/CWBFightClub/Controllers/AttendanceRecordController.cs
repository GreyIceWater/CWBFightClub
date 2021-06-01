using CWBFightClub.Data;
using CWBFightClub.Models;
using CWBFightClub.Services;
using CWBFightClub.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CWBFightClub.Controllers
{
    public class AttendanceRecordController : BaseController
    {
        public AttendanceRecordController(CWBContext db, IAccessChecker ac)
            : base(ac, db)
        {
        }

        [HttpGet("/Checkin")]
        public async Task<IActionResult> Checkin(string searchString, int pageNumber = 1, int pageNumber2 = 1)
        {
            // ViewBags: ActiveScheduledClasses, EnrolledStudents, NotEnrolledStudents, HasActiveScheduledClasses, HasEnrolledStudents, HasNotEnrolledStudents
            await PopulateAttendanceEntryLists(DateTime.Now, searchString, pageNumber, pageNumber2);

            return View();
        }

        /// <summary>
        /// Generate ViewBag data for attendance entry lists.
        /// ViewBags: ActiveScheduledClasses, EnrolledStudents, NotEnrolledStudents, HasActiveScheduledClasses, HasEnrolledStudents, HasNotEnrolledStudents
        /// This method sets the HasX ViewBags. Other ViewBags are set in their respective methods.
        /// </summary>
        /// <param name="searchString">The current search string from user input.</param>
        /// <param name="pageNumber">The current page number for the list of enrolled students.</param>
        /// <param name="pageNumber2">The current page number for the list of not-enrolled students.</param>
        private async Task<bool> PopulateAttendanceEntryLists(DateTime now, string searchString, int pageNumber, int pageNumber2)
        {
            try
            {
                List<ScheduledClass> activeScheduledClasses = await PopulateActiveScheduledClasses();
                ViewBag.HasActiveScheduledClasses = activeScheduledClasses.Any();

                List<Student> enrolledStudents = await PopulateEnrolledStudents(activeScheduledClasses.Select(x => x.DisciplineID), searchString, pageNumber);
                ViewBag.HasEnrolledStudents = enrolledStudents.Any();

                ViewBag.HasNotEnrolledStudents = await PopulateNotEnrolledStudents(enrolledStudents, searchString, pageNumber2);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Generates a viewbag with select option elements for active scheduled classes.
        /// </summary>
        /// <returns>A list of active scheduled classes.</returns>
        private async Task<List<ScheduledClass>> PopulateActiveScheduledClasses()
        {
            // https://stackoverflow.com/questions/22258070/datetime-dayofweek-micro-optimization/22278311#22278311
            // above link is a good starting point for optimizing these day of week checks.
            List<ScheduledClass> nonarchivedScheduledClasses = await _db.ScheduledClasses
                .Where(x => !x.IsArchived && x.Name != SystemConstants.Walkin)
                .OrderBy(x => x.Name).ToListAsync();

            List<ScheduledClass> activeScheduledClasses = nonarchivedScheduledClasses
                .Where(x => x.Start.DayOfWeek == DateTime.Now.DayOfWeek &&
                x.Start.TimeOfDay.Subtract(new TimeSpan(0, SystemConstants.MinutesAllowedForCheckinEarly, 0)) < DateTime.Now.TimeOfDay &&
                x.End.TimeOfDay > DateTime.Now.TimeOfDay &&
                x.End.AddDays(x.RecurrenceTime * 7) > DateTime.Now).ToList();

            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (ScheduledClass s in activeScheduledClasses)
            {
                selectList.Add(new SelectListItem
                {
                    Value = s.ScheduledClassID.ToString(),
                    Text = s.Name
                });
            }

            int value = _db.ScheduledClasses.Where(x => x.Name == SystemConstants.Walkin).Select(x => x.ScheduledClassID).FirstOrDefault();

            // Add walkin option to the bottom of the list.
            selectList.Add(new SelectListItem
            {
                Value = value.ToString(),
                Text = SystemConstants.Walkin
            });

            ViewBag.ActiveScheduledClasses = selectList;

            return activeScheduledClasses;
        }

        /// <summary>
        /// Populates the enrolled students list and view bag.
        /// </summary>
        /// <param name="currentScheduledDisciplineIDs">The list of IDs of currently scheduled disciplines.</param>
        /// <param name="searchString">The current search string from user input.</param>
        /// <param name="pageNumber">The current page number of the enrolled students list.</param>
        /// <returns>The list of enrolled students.</returns>
        private async Task<List<Student>> PopulateEnrolledStudents(IEnumerable<int> currentScheduledDisciplineIDs, string searchString, int pageNumber)
        {
            List<Student> students = await _db.Students
                .Include(x => x.Enrollments.Where(x => x.IsArchived == false && (!x.EndDate.HasValue || x.EndDate > DateTime.Now)))
                .ThenInclude(x => x.Discipline).ToListAsync();

            List<Student> enrolledStudents = students
                .Where(x => x.IsArchived == false && x.Enrollments.Any(x => currentScheduledDisciplineIDs.Contains(x.DisciplineID)))
                .OrderBy(x => x.LastName)
                .ToList();

            await AssignCheckinStatus(enrolledStudents);

            ViewData["CurrentSearch"] = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                enrolledStudents = enrolledStudents.Where(a => a.FirstName.ToLower().Contains(searchString.ToLower()) ||
                            a.LastName.ToLower().Contains(searchString.ToLower())).ToList();
            }

            ViewBag.EnrolledStudents = enrolledStudents;
            PaginatedList<Student> pList = PaginatedList<Student>.Create(enrolledStudents.AsQueryable(), pageNumber, SystemConstants.ItemsPerPage);
            ViewBag.HasPreviousPage = pList.HasPreviousPage;
            ViewBag.HasNextPage = pList.HasNextPage;
            ViewBag.PageIndex = pList.PageIndex;
            ViewBag.TotalPages = pList.TotalPages;



            ViewBag.EnrolledStudents = pList;
            return enrolledStudents;
        }

        /// <summary>
        /// Generates a viewbag with select option elements for not enrolled students.
        /// </summary>
        /// <param name="enrolledStudents">The list of enrolled students in active scheduled classes.</param>
        /// <param name="searchString">The current search string from user input.</param>
        /// <param name="pageNumber2">The current page number of the not-enrolled students list.</param>
        /// <returns>True if not enrolled students exist.</returns>
        private async Task<bool> PopulateNotEnrolledStudents(List<Student> enrolledStudents, string searchString, int pageNumber2)
        {
            List<Student> notEnrolledStudents = await _db.Students.Where(x => x.IsArchived == false && x.FirstName != "Admin" && !enrolledStudents.Contains(x))
                                                            .OrderBy(x => x.LastName).ToListAsync();

            if (!notEnrolledStudents.Any())
            {
                return false;
            }

            await AssignCheckinStatus(notEnrolledStudents);

            ViewData["CurrentSearch"] = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                notEnrolledStudents = notEnrolledStudents.Where(a => a.FirstName.ToLower().Contains(searchString.ToLower()) ||
                            a.LastName.ToLower().Contains(searchString.ToLower())).ToList();
            }

            PaginatedList<Student> pList = PaginatedList<Student>.Create(notEnrolledStudents.AsQueryable(), pageNumber2, SystemConstants.ItemsPerPage);
            ViewBag.HasPreviousPage2 = pList.HasPreviousPage;
            ViewBag.HasNextPage2 = pList.HasNextPage;
            ViewBag.PageIndex2 = pList.PageIndex;
            ViewBag.TotalPages2 = pList.TotalPages;

            // ViewBag: NotEnrolledStudents
            ViewBag.NotEnrolledStudents = pList;

            /* For select options / combobox entry;
            if (notEnrolledStudents.Any())
            {
                await AssignCheckinStatus(notEnrolledStudents);

                ViewBag.NotEnrolledStudents = notEnrolledStudents.Select(student => new SelectListItem
                {
                    Value = student.StudentID.ToString(),
                    Text = $"{student.LastName}, {student.FirstName}"
                });

                return true;
            }
            */

            return true;
        }

        /// <summary>
        /// Assigned checked in status of a list of students.
        /// </summary>
        /// <param name="students">The list of students to assign check in status.</param>
        /// <returns>True if successful.</returns>
        private async Task<bool> AssignCheckinStatus(List<Student> students)
        {
            try
            {
                foreach (Student s in students)
                {
                    await IsCheckedIn(s);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Assigns a creator of -1 to new records when created by system.
        /// </summary>
        /// <param name="record">The record to assign creator properties to.</param>
        private static void AssignCreatorAsSystem(AttendanceRecord record)
        {
            record.CreatedBy = -1;
            record.CreatedDate = DateTime.Now;
        }

        /// <summary>
        /// Assigns a modifier of -1 to records modified by system.
        /// </summary>
        /// <param name="record">The record to assign modifier properties to.</param>
        private static void AssignModifierAsSystem(AttendanceRecord record)
        {
            record.ModifiedBy = -1;
            record.ModifiedDate = DateTime.Now;
        }

        /// <summary>
        /// Checks a student in to a class.
        /// </summary>
        /// <param name="record">The record to set properties of and add to db.</param>
        /// <returns>Redirects to Get method Checkin.</returns>
        [HttpPost("/Checkin")]
        public IActionResult Checkin(AttendanceRecord record)
        {
            if (record is null)
            {
                return Error();
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Checkin");
            }

            // Set record end to the end of class. Can be ended early.
            ScheduledClass sc = _db.ScheduledClasses.Where(x => x.ScheduledClassID == record.ScheduledClassID).FirstOrDefault();
            TimeSpan timeDifference = sc.End.TimeOfDay - DateTime.Now.TimeOfDay;
            DateTime endDateTime = DateTime.Now.Add(timeDifference);

            AssignCreatorAsSystem(record);
            record.IsVerified = false;
            record.Start = DateTime.Now;
            record.End = endDateTime;

            _db.AttendanceRecords.Add(record);
            _db.SaveChanges();


            return RedirectToAction("Checkin");
        }

        /// <summary>
        /// Prematurely sets the end datetime of an attendance record.
        /// </summary>
        /// <param name="record">The record to set the end datetime on.</param>
        /// <returns>Redirects to checkin page.</returns>
        [HttpPost]
        public IActionResult Checkout(AttendanceRecord record)
        {
            if (record is null)
            {
                return Error();
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Checkin");
            }

            AttendanceRecord recordFound = _db.AttendanceRecords
                .Where(x => x.IsArchived == false && x.StudentID == record.StudentID && x.ScheduledClassID == record.ScheduledClassID && x.End > DateTime.Now).FirstOrDefault();

            if (recordFound != null)
            {
                AssignModifierAsSystem(recordFound);
                recordFound.End = DateTime.Now;

                _db.AttendanceRecords.Update(recordFound);
                _db.SaveChanges();
            }

            return RedirectToAction("Checkin");
        }

        // Standard Get method for delete.
        public async Task<IActionResult> Delete(int? id)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (id is null)
            {
                return RedirectToAction("ObjectNotFound", new { type = typeof(AttendanceRecord).Name, message = "Deletion failed." });
            }

            AttendanceRecord record = await _db.FindAsync<AttendanceRecord>(id);

            if (record is null || record.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", new { type = typeof(AttendanceRecord).Name, message = "Deletion failed." });
            }

            record = await _db.AttendanceRecords
                .Where(x => x.AttendanceRecordID == id)
                .Include(x => x.Student)
                .Include(x => x.ScheduledClass)
                .FirstOrDefaultAsync();

            return View(record);
        }

        // Standard Post method for delete.
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePost(AttendanceRecord record)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            AttendanceRecord recordFound = await _db.FindAsync<AttendanceRecord>(record.AttendanceRecordID);

            if (recordFound is null || recordFound.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", new { type = typeof(AttendanceRecord).Name, message = "Deletion failed." });
            }

            AssignModifier(recordFound);
            recordFound.IsArchived = true;
            _db.AttendanceRecords.Update(recordFound);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        // Standard Get method for edit.
        public async Task<IActionResult> Edit(int? id)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (id == null)
            {
                return RedirectToAction("ObjectNotFound", new { type = typeof(AttendanceRecord).Name, message = "Edit failed." });
            }

            AttendanceRecord record = await _db.FindAsync<AttendanceRecord>(id);

            if (record == null || record.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", new { type = typeof(AttendanceRecord).Name, message = "Edit failed." });
            }

            await PopulateClassAndStudentViewBags();

            return View(record);
        }

        // Standard Post method for edit.
        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditPost(AttendanceRecord record)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            AttendanceRecord recordFound = await _db.AttendanceRecords.FindAsync(record.AttendanceRecordID);

            if (record is null || record.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", new { type = typeof(AttendanceRecord).Name, message = "Edit failed." });
            }

            if (!ModelState.IsValid)
            {
                await PopulateClassAndStudentViewBags();
                return View(record);
            }

            if (!await TryUpdateModelAsync(
                recordFound,
                "",
                x => x.ScheduledClassID,
                x => x.StudentID,
                x => x.Start,
                x => x.End,
                x => x.IsVerified))
            {
                await PopulateClassAndStudentViewBags();
                return View(record);
            }

            AssignModifier(recordFound);

            _db.AttendanceRecords.Update(recordFound);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Standard index page listing all attendance records.
        /// </summary>
        /// <param name="sortOrder">The current sort order of the page.</param>
        /// <param name="searchString">The current search string from user input.</param>
        /// <param name="pageNumber">The current page number of the list of records.</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber = 1)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            IEnumerable<AttendanceRecord> records = null;

            await Task.Run(() =>
            {
                records = _db.AttendanceRecords
                                     .Where(x => x.IsArchived == false)
                                     .Include(x => x.ScheduledClass)
                                     .ThenInclude(scheduleClass => scheduleClass.Discipline)
                                     .Include(x => x.Student)
                                     .AsNoTracking();
            });

            // ScheduledClasses and Students viewbags for create.
            ViewBag.ClassesAndStudentsExist = await PopulateClassAndStudentViewBags();

            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentSearch"] = searchString;

            ViewData["StartSortParm"] = sortOrder == "Start" ? "start_desc" : "Start";
            ViewData["EndSortParm"] = sortOrder == "End" ? "end_desc" : "End";
            ViewData["StudentLastNameSortParm"] = sortOrder == "StudentLastName" ? "studentlastname_desc" : "StudentLastName";
            ViewData["ClassNameSortParm"] = sortOrder == "ClassName" ? "classname_desc" : "ClassName";
            ViewData["DisciplineSortParm"] = sortOrder == "Discipline" ? "discipline_desc" : "Discipline";

            switch (sortOrder)
            {
                case "Start":
                    records = records.OrderBy(r => r.Start);
                    break;
                case "start_desc":
                    records = records.OrderByDescending(r => r.Start);
                    break;
                case "End":
                    records = records.OrderBy(r => r.End);
                    break;
                case "end_desc":
                    records = records.OrderByDescending(r => r.End);
                    break;
                case "StudentLastName":
                    records = records.OrderBy(r => r.Student.LastName);
                    break;
                case "studentlastname_desc":
                    records = records.OrderByDescending(r => r.Student.LastName);
                    break;
                case "ClassName":
                    records = records.OrderBy(r => r.ScheduledClass.Name);
                    break;
                case "classname_desc":
                    records = records.OrderByDescending(r => r.ScheduledClass.Name);
                    break;
                case "Discipline":
                    records = records.OrderBy(r => r.ScheduledClass.Discipline.Name);
                    break;
                case "discipline_desc":
                    records = records.OrderByDescending(r => r.ScheduledClass.Discipline.Name);
                    break;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                records = records.Where(r => r.Student.FirstName.ToLower().Contains(searchString.ToLower()) ||
                            r.Student.LastName.ToLower().Contains(searchString.ToLower()));
            }

            return View(PaginatedList<AttendanceRecord>.Create(records.AsQueryable(), pageNumber.Value, SystemConstants.ItemsPerPage));
        }

        /// <summary>
        /// Populates the student and scheduled class lists for add and edit.
        /// </summary>
        /// <returns>True if successful.</returns>
        private async Task<bool> PopulateClassAndStudentViewBags()
        {
            List<ScheduledClass> scheduledClasses = await _db.ScheduledClasses.Where(x => !x.IsArchived).ToListAsync();

            List<Student> students = await _db.Students.Where(x => !x.IsArchived && !x.FirstName.Contains("Admin")).ToListAsync();


            if (scheduledClasses.Any() && students.Any())
            {
                ViewBag.ScheduledClasses = GenerateClassSelectList(scheduledClasses);
                ViewBag.Students = GenerateStudentSelectList(students);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Generates select item lists for a list of students.
        /// </summary>
        /// <param name="students">The list of students to convert.</param>
        /// <returns>The list of list items.</returns>
        private List<SelectListItem> GenerateStudentSelectList(List<Student> students)
        {
            var selectList = new List<SelectListItem>();

            foreach (Student s in students)
            {
                selectList.Add(
                    new SelectListItem { Text = $"{s.FirstName} {s.LastName}", Value = s.StudentID.ToString() }
                );
            }

            return selectList;
        }

        /// <summary>
        /// Generates select item lists for a list of classes.
        /// </summary>
        /// <param name="students">The list of classes to convert.</param>
        /// <returns>The list of list items.</returns>
        private List<SelectListItem> GenerateClassSelectList(List<ScheduledClass> classes)
        {
            var selectList = new List<SelectListItem>();

            foreach (ScheduledClass s in classes)
            {
                selectList.Add(
                    new SelectListItem { Text = s.Name, Value = s.ScheduledClassID.ToString() }
                );
            }

            return selectList;
        }

        // Standard Post method for create.
        [HttpPost]
        public async Task<IActionResult> Create(AttendanceRecord record)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (!ModelState.IsValid)
            {
                return PartialView("_Create", record);
            }

            try
            {
                AssignCreator(record);

                await _db.AttendanceRecords.AddAsync(record);
                _db.SaveChanges();
            }
            catch (Exception)
            {
                Debug.Write(ModelState);
                return RedirectToAction("Index");
            }

            return PartialView("_AddSuccess");
        }

        /// <summary>
        /// Determines if a student is checked in already.
        /// </summary>
        /// <param name="student">The student to check.</param>
        private async Task IsCheckedIn(Student student)
        {
            AttendanceRecord attendanceRecord = await _db.AttendanceRecords
                .Where(x => x.IsArchived == false && x.StudentID == student.StudentID && x.End > DateTime.Now).FirstOrDefaultAsync();

            if (attendanceRecord == null)
            {
                student.IsCheckedIn = false;

                return;
            }

            attendanceRecord = await _db.AttendanceRecords
                .Where(x => x.IsArchived == false && x.StudentID == student.StudentID && x.End > DateTime.Now)
                .Include(x => x.ScheduledClass)
                .FirstOrDefaultAsync();

            student.IsCheckedIn = true;
            student.ActiveAttendanceRecord = attendanceRecord;
        }
    }
}
