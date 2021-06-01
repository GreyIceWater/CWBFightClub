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
    public class EnrollmentController : BaseController
    {
        private static IEnrollmentUtility _enrollmentUtility;
        private static IStudentUtility _studentUtility;
        public EnrollmentController(CWBContext db, IAccessChecker ac, IEnrollmentUtility enrollmentUtility, IStudentUtility studentUtility)
            : base(ac, db)
        {
            _enrollmentUtility = enrollmentUtility;
            _studentUtility = studentUtility;
        }

        // Create Get method to be used by administrators.
        public async Task<IActionResult> Create(int? studentID = null)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            // Set viewbag variable if user is or is not logged in.
            if (checkResult != null)
            {
                ViewBag.noLogin = true;
            }
            else
            {
                ViewBag.noLogin = false;
            }

            Enrollment enrollment = new Enrollment()
            {
                StartDate = DateTime.Now
            };

            if (studentID is not null)
            {
                enrollment.StudentID = studentID.Value;
                ViewBag.DisciplinesExist = await PopulateDisciplinesAvailableForStudent(studentID.Value);
            }

            ViewData["StudentID"] = studentID;

            return PartialView("_Create", enrollment);
        }

        // Create Post method to be used by administrators.
        [HttpPost]
        public IActionResult Create(Enrollment enrollment)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();

            if (enrollment is null)
            {
                return Error();
            }

            if (!ModelState.IsValid)
            {
                return View(enrollment);
            }

            AssignCreator(enrollment);

            _db.Enrollments.Add(enrollment);
            _db.SaveChanges();

            // Set viewbag variable if user is or is not logged in and return to appropriate view.
            if (checkResult != null)
            {
                ViewBag.noLogin = true;
                return RedirectToAction("Checkin", "AttendanceRecord");
            }
            else
            {
                ViewBag.noLogin = false;
                return RedirectToAction("Student", new { id = enrollment.StudentID });
            }
        }

        // Generates delete confirm page. Be sure user wants to actually delete and not stop enrollment.
        // Delete archives this enrollment and all achieved belts associated with it. Stop enrollment just stops payment plan and progress.
        public async Task<IActionResult> Delete(int? id)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            Enrollment enrollment = await _db.FindAsync<Enrollment>(id);

            if (enrollment is null || enrollment.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", new { type = typeof(Enrollment).Name, message = "Deletion failed." });
            }

            Enrollment enrollmentExists = await _db.Enrollments.Include(x => x.Student)
                                .Include(x => x.Discipline)
                                .Include(x => x.AchievedBelts.Where(y => y.IsArchived == false))
                                .AsNoTracking()
                                .SingleAsync(x => x.EnrollmentID == id.Value);

            TempData["FromStudentWorkflow"] = TempData["FromStudentWorkflow"];

            return View(enrollmentExists);
        }

        // Post method for delete. Be sure user wants to actually delete and not stop enrollment.
        // Delete archives this enrollment and all achieved belts associated with it. Stop enrollment just stops payment plan and progress.
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePost(Enrollment enrollmentFromPost, bool fromStudentWorkflow = false)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            Enrollment enrollment = await _db.FindAsync<Enrollment>(enrollmentFromPost.EnrollmentID);

            if (enrollment is null || enrollment.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", new { type = typeof(Enrollment).Name, message = "Deletion failed." });
            }

            Enrollment enrollmentFound = await _db.Enrollments.Include(x => x.AchievedBelts).SingleAsync(x => x.EnrollmentID == enrollment.EnrollmentID);

            foreach (AchievedBelt ab in enrollmentFound.AchievedBelts)
            {
                ab.IsArchived = true;
                _db.AchievedBelts.Update(ab);
            }

            Student student = await _db.Students.Where(x => x.StudentID == enrollment.StudentID).FirstOrDefaultAsync();
            _studentUtility.UpdateStudentBalance(student, _db);
            Discipline discipline = await _db.Disciplines.Where(x => x.DisciplineID == enrollment.DisciplineID).FirstOrDefaultAsync();

            student.PaymentAgreementAmount = student.PaymentAgreementAmount.HasValue ? student.PaymentAgreementAmount - discipline.DefaultCostPerMonth : 0;
            if (student.PaymentAgreementAmount < 0)
            {
                student.PaymentAgreementAmount = 0m;
            }

            if (student.Enrollments.Where(x => !x.IsArchived && x.EndDate > DateTime.Now).Count() == 0)
            {
                student.BalanceDueDate = null;
            }

            _db.Students.Update(student);

            AssignModifier(enrollmentFound);
            enrollmentFound.IsArchived = true;
            _db.Enrollments.Update(enrollmentFound);
            _db.SaveChanges();

            return RedirectToAction("Student", new { id = enrollment.StudentID, fromStudentWorkflow });
        }

        // Generates edit page partial view.
        public async Task<IActionResult> Edit(int? id)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            Enrollment enrollment = await _db.FindAsync<Enrollment>(id);

            if (enrollment == null || enrollment.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", new { type = typeof(Enrollment).Name, message = "Edit failed." });
            }

            Enrollment enrollmentFound = await _db.Enrollments.Include(x => x.Discipline).SingleAsync(x => x.EnrollmentID == id);
            TempData["FromStudentWorkflow"] = TempData["FromStudentWorkflow"];
            return PartialView("_Edit", enrollment);
        }

        // Post method for edit.
        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditPost(Enrollment en)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            Enrollment enrollment = await _db.Enrollments.FindAsync(en.EnrollmentID);

            if (enrollment is null || enrollment.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", new { type = typeof(Enrollment).Name, message = "Edit failed." });
            }
            TempData["FromStudentWorkflow"] = TempData["FromStudentWorkflow"];
            if (!ModelState.IsValid)
            {
                return PartialView(enrollment);
            }

            Student student = await _db.Students.Where(x => x.StudentID == enrollment.StudentID).FirstOrDefaultAsync();
            _studentUtility.UpdateStudentBalance(student, _db);

            enrollment.StartDate = en.StartDate;
            enrollment.EndDate = en.EndDate;

            AssignModifier(enrollment);

            _db.Enrollments.Update(enrollment);
            _db.SaveChanges();

            string previousUrl = ViewData["PreviousUrl"] as string;
            bool fromStudentWorkflow = TempData["FromStudentWorkflow"] != null ? (bool)TempData["FromStudentWorkflow"] : false;

            return RedirectToAction("Student", new { id = enrollment.StudentID, fromStudentWorkflow });
        }

        // Generates details partial view.
        public async Task<IActionResult> Detail(int? id)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            Enrollment enrollment = await _db.FindAsync<Enrollment>(id);

            if (enrollment == null || enrollment.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", new { type = typeof(Enrollment).Name, message = "Edit failed." });
            }

            Enrollment enrollmentFound = await _db.Enrollments.Include(x => x.Discipline)
                .Include(x => x.AchievedBelts.Where(x => !x.IsArchived))
                .Include(x => x.Student)
                .SingleAsync(x => x.EnrollmentID == id);
            TempData["StudentID"] = enrollmentFound.StudentID;

            ViewBag.NextBeltExists = await _enrollmentUtility.NextBeltExists(enrollmentFound.EnrollmentID);

            return PartialView("_Detail", enrollment);
        }

        // Restart an enrollment by setting end date to null.
        public async Task<IActionResult> RestartStudentEnrollment(int id, bool fromDiscipline = false)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            Enrollment enrollment = await _db.Enrollments.FindAsync(id);

            if (enrollment is null || enrollment.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", new { type = typeof(Enrollment).Name, message = "Edit failed." });
            }

            Student student = await _db.Students.Where(x => x.StudentID == enrollment.StudentID).FirstOrDefaultAsync();
            Discipline discipline = await _db.Disciplines.Where(x => x.DisciplineID == enrollment.DisciplineID).FirstOrDefaultAsync();

            student.PaymentAgreementAmount = student.PaymentAgreementAmount.HasValue ? student.PaymentAgreementAmount + discipline.DefaultCostPerMonth : discipline.DefaultCostPerMonth;
            student.BalanceDueDate = student.BalanceDue.HasValue ? student.BalanceDueDate : DateTime.Now;

            _db.Students.Update(student);

            enrollment.EndDate = null;
            AssignModifier(enrollment);
            _db.Enrollments.Update(enrollment);
            _db.SaveChanges();

            bool fromStudentWorkflow = TempData["FromStudentWorkflow"] != null ? (bool)TempData["FromStudentWorkflow"] : false;

            if (fromDiscipline)
            {
                return RedirectToAction("Discipline", new { id = enrollment.DisciplineID });
            }

            return RedirectToAction("Student", new { id = enrollment.StudentID, fromStudentWorkflow });
        }

        // Restart an enrollment from the discipline area.
        public async Task<IActionResult> RestartStudentEnrollmentFromDisc(int id)
        {
            return await RestartStudentEnrollment(id, true);
        }

        // Stops an enrollment by setting end date to now.
        public async Task<IActionResult> CancelStudentEnrollment(int id)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            Enrollment enrollment = await _db.Enrollments.FindAsync(id);

            if (enrollment is null || enrollment.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", new { type = typeof(Enrollment).Name, message = "Edit failed." });
            }

            Student student = await _db.Students.Where(x => x.StudentID == enrollment.StudentID).FirstOrDefaultAsync();
            _studentUtility.UpdateStudentBalance(student, _db);
            Discipline discipline = await _db.Disciplines.Where(x => x.DisciplineID == enrollment.DisciplineID).FirstOrDefaultAsync();

            student.PaymentAgreementAmount = student.PaymentAgreementAmount.HasValue ? student.PaymentAgreementAmount - discipline.DefaultCostPerMonth : 0;
            if (student.PaymentAgreementAmount < 0)
            {
                student.PaymentAgreementAmount = 0m;
            }

            if (student.Enrollments.Where(x => !x.IsArchived && x.EndDate > DateTime.Now).Count() == 0)
            {
                student.BalanceDueDate = null;
            }

            _db.Students.Update(student);

            enrollment.EndDate = DateTime.Now;
            AssignModifier(enrollment);
            _db.Enrollments.Update(enrollment);
            _db.SaveChanges();

            return RedirectToAction("Discipline", new { id = enrollment.DisciplineID });
        }

        // Adds first available rank
        private async Task AddFirstRankAchievedBelt(int enrollmentID)
        {
            Belt nextBelt = await _enrollmentUtility.NextBeltIs(enrollmentID);

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
        }

        // Adds enrollment and modifies payment agreement amounts to default values defined on discipline model or app settings model.
        private async Task<bool> Add(int disciplineID, DateTime startDate, int studentID)
        {
            try
            {
                if (studentID > 0 && disciplineID > 0)
                {
                    Student student = await _db.Students.FindAsync(studentID);
                    _studentUtility.UpdateStudentBalance(student, _db);
                    Discipline discipline = await _db.Disciplines.FindAsync(disciplineID);

                    if (student.PaymentAgreenmentPeriod == PaymentPeriod.Monthly || student.PaymentAgreenmentPeriod == 0)
                    {
                        if (student.PaymentAgreementAmount.HasValue)
                        {
                            student.PaymentAgreementAmount += discipline.DefaultCostPerMonth;
                            student.PaymentAgreenmentPeriod = PaymentPeriod.Monthly;
                        }
                        else
                        {
                            student.PaymentAgreementAmount = discipline.DefaultCostPerMonth;
                        }

                        student.BalanceDue = student.BalanceDue.HasValue ? student.BalanceDue + discipline.DefaultCostPerMonth : discipline.DefaultCostPerMonth;
                        student.BalanceDueDate = student.BalanceDueDate.HasValue ? student.BalanceDueDate : DateTime.Now.AddMonths(1);
                    }
                    else if (student.PaymentAgreenmentPeriod == PaymentPeriod.ThreeMonth)
                    {
                        if (student.PaymentAgreementAmount.HasValue)
                        {
                            student.PaymentAgreementAmount += discipline.DefaultCostPerMonth * 3;
                        }
                        else
                        {
                            student.PaymentAgreementAmount = discipline.DefaultCostPerMonth * 3;
                        }

                        student.BalanceDue = student.BalanceDue.HasValue ? student.BalanceDue + discipline.DefaultCostPerMonth * 3 : discipline.DefaultCostPerMonth * 3;
                        student.BalanceDueDate = student.BalanceDueDate.HasValue ? student.BalanceDueDate : DateTime.Now.AddMonths(3);
                    }

                    else if (student.PaymentAgreenmentPeriod == PaymentPeriod.Yearly)
                    {
                        if (student.PaymentAgreementAmount.HasValue)
                        {
                            student.PaymentAgreementAmount += discipline.DefaultCostPerMonth * 12;
                        }
                        else
                        {
                            student.PaymentAgreementAmount = discipline.DefaultCostPerMonth * 12;
                        }

                        student.BalanceDue = student.BalanceDue.HasValue ? student.BalanceDue + discipline.DefaultCostPerMonth * 12 : discipline.DefaultCostPerMonth * 12;
                        student.BalanceDueDate = student.BalanceDueDate.HasValue ? student.BalanceDueDate : DateTime.Now.AddMonths(12);
                    }

                    _db.Students.Update(student);

                    Enrollment enrollment = new Enrollment()
                    {
                        DisciplineID = disciplineID,
                        StudentID = studentID,
                        StartDate = startDate,
                        EndDate = null
                    };

                    AssignCreator(enrollment);

                    await _db.Enrollments.AddAsync(enrollment);
                    _db.SaveChanges();
                    await AddFirstRankAchievedBelt(enrollment.EnrollmentID);

                }
                else if (studentID > 0 && disciplineID < 0)
                {
                    await SelectPackage(studentID, disciplineID);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Perform a create from discipline using drop down menu.
        public async Task<IActionResult> AddFromDiscipline(int disciplineID, DateTime startDate, int studentID)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            await Add(disciplineID, startDate, studentID);



            return RedirectToAction("Discipline", new { id = disciplineID });
        }

        // Perform a create from student.
        public async Task<IActionResult> AddFromStudent(int disciplineID, DateTime startDate, int studentID, bool fromStudentWorkflow = false)
        {
            Guid guid = Guid.NewGuid();
            TempData["SecurityCheck"] = guid;

            await Add(disciplineID, startDate, studentID);
            return RedirectToAction("Student", new { id = studentID, fromStudentWorkflow });
        }

        // List of enrolled students for a discipline. id = discipline ID.
        public async Task<IActionResult> Discipline(int id, string searchString, int pageNumber = 1, int pageNumber2 = 1)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            Discipline discipline = null;

            discipline = await _db.Disciplines
                .Where(x => x.IsArchived == false && x.DisciplineID == id)
                .Include(x => x.Enrollments.Where(y => y.IsArchived == false))
                .ThenInclude(x => x.Student)
                .AsNoTracking().FirstOrDefaultAsync();

            //ViewBag.Students is created here.
            ViewBag.StudentsExist = await PopulateStudentsAvailableForDiscipline(id);

            List<Enrollment> activeEnrollments = discipline.Enrollments.Where(x => x.EndDate == null || x.EndDate >= DateTime.Now).ToList();
            List<Enrollment> inactiveEnrollments = discipline.Enrollments.Where(x => x.EndDate != null && x.EndDate < DateTime.Now).ToList();
            
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                activeEnrollments = activeEnrollments
                    .Where(x => x.Student.FirstName.ToLower().Contains(searchString.ToLower()) || x.Student.LastName.ToLower().Contains(searchString.ToLower())).ToList();
                inactiveEnrollments = inactiveEnrollments
                    .Where(x => x.Student.FirstName.ToLower().Contains(searchString.ToLower()) || x.Student.LastName.ToLower().Contains(searchString.ToLower())).ToList();
            }

            ViewData["CurrentSearch"] = searchString;

            PaginatedList<Enrollment> pList = PaginatedList<Enrollment>.Create(activeEnrollments.AsQueryable(), pageNumber, SystemConstants.ItemsPerPage);
            ViewBag.HasPreviousPage = pList.HasPreviousPage;
            ViewBag.HasNextPage = pList.HasNextPage;
            ViewBag.PageIndex = pList.PageIndex;
            ViewBag.TotalPages = pList.TotalPages;

            PaginatedList<Enrollment> pList2 = PaginatedList<Enrollment>.Create(inactiveEnrollments.AsQueryable(), pageNumber2, SystemConstants.ItemsPerPage);
            ViewBag.HasPreviousPage2 = pList2.HasPreviousPage;
            ViewBag.HasNextPage2 = pList2.HasNextPage;
            ViewBag.PageIndex2 = pList2.PageIndex;
            ViewBag.TotalPages2 = pList2.TotalPages;

            ViewBag.NotEnrolledStudents = pList2;
            ViewBag.EnrolledStudents = pList;

            return View(discipline);
        }

        // List of enrolled disciplines for a student. id = student ID.
        public async Task<IActionResult> Student(int id, int enrollmentID = 0, bool fromStudentWorkflow = false)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            ViewBag.noLogin = checkResult == null ? false : true;
            
            // Check to see if the guid was set to TempData, ensuring user intentions are pure.
            // If users are doing any URL manipulation or try accessing without first creating a new user, they are kicked out.
            if (checkResult != null && TempData["SecurityCheck"] == null)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = "Student", message = "Busted! Invalid operation." });
            }

            Student student = null;

            student = await _db.Students
                                    .Where(x => x.IsArchived == false && x.StudentID == id)
                                    .Include(x => x.Enrollments.Where(y => y.IsArchived == false))
                                    .ThenInclude(x => x.AchievedBelts.Where(y => y.IsArchived == false))
                                    .Include(x => x.Enrollments.Where(y => y.IsArchived == false))
                                    .ThenInclude(x => x.Discipline)
                                    .AsNoTracking().FirstOrDefaultAsync();

            ViewBag.EnrollmentDetail = enrollmentID;

            //ViewBag.Disciplines is created here.
            ViewBag.DisciplinesExist = await PopulateDisciplinesAvailableForStudent(id);
            ViewBag.FromStudentWorkflow = fromStudentWorkflow;
            TempData["FromStudentWorkflow"] = fromStudentWorkflow;

            return View(student);
        }

        // Determines if enrollments exist when the gym package is selected. Enrolls in all disciplines that aren't already enrolled in.
        // Restarts enrollments that were stopped. Sets payment agreement amount to default for gym package.
        private async Task SelectPackage(int studentID, int package)
        {

            Student student = await _db.Students.Where(x => x.StudentID == studentID)
                .Include(x => x.Enrollments.Where(x => !x.IsArchived)).FirstOrDefaultAsync();
            List<int> disciplines = await _db.Disciplines.Where(x => !x.IsArchived && x.Name != SystemConstants.Walkin).Select(x => x.DisciplineID).ToListAsync();

            List<int> disciplineIDsToAdd = new List<int>();

            foreach (int d in disciplines)
            {
                bool foundEnrollment = false;
                foreach (Enrollment e in student.Enrollments)
                {
                    if (d == e.DisciplineID)
                    {
                        e.EndDate = null;
                        foundEnrollment = true;
                        _db.Enrollments.Update(e);
                    }
                    else
                    {
                        e.StartDate = DateTime.Now;
                        _db.Enrollments.Update(e);
                    }
                }

                if (foundEnrollment == false)
                {
                    disciplineIDsToAdd.Add(d);
                }
            }

            foreach (int d in disciplineIDsToAdd)
            {
                Enrollment enrollment = new Enrollment()
                {
                    StudentID = studentID,
                    DisciplineID = d,
                    StartDate = DateTime.Now
                };

                _db.Enrollments.Add(enrollment);
                _db.SaveChanges();
                await AddFirstRankAchievedBelt(enrollment.EnrollmentID);
            }


            AppSetting appSetting = await _db.AppSettings.Where(x => x.AppSettingID > 0).FirstOrDefaultAsync();

            switch (package)
            {
                case -1:
                    student.PaymentAgreementAmount = appSetting.BundleCostPerMonth;
                    student.BalanceDue = student.BalanceDue.HasValue ? student.BalanceDue + appSetting.BundleCostPerMonth : appSetting.BundleCostPerMonth;
                    student.PaymentAgreenmentPeriod = PaymentPeriod.Monthly;
                    student.BalanceDueDate = student.BalanceDueDate.HasValue ? student.BalanceDueDate : DateTime.Now.AddMonths(1);
                    break;
                case -2:
                    student.PaymentAgreementAmount = appSetting.BundleCostPerThreeMonths;
                    student.BalanceDue += appSetting.BundleCostPerThreeMonths;
                    student.PaymentAgreenmentPeriod = PaymentPeriod.ThreeMonth;
                    student.BalanceDueDate = student.BalanceDueDate.HasValue ? student.BalanceDueDate : DateTime.Now.AddMonths(3);
                    break;
                case -3:
                    student.PaymentAgreementAmount = appSetting.BundleCostPerYear;
                    student.BalanceDue += appSetting.BundleCostPerYear;
                    student.PaymentAgreenmentPeriod = PaymentPeriod.Yearly;
                    student.BalanceDueDate = student.BalanceDueDate.HasValue ? student.BalanceDueDate : DateTime.Now.AddMonths(12);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unknown Gym Membership");
            }

            _db.Students.Update(student);
            _db.SaveChanges();
        }

        // Generates ViewBag.Students with students that are available to enroll in a discipline.
        private async Task<bool> PopulateStudentsAvailableForDiscipline(int disciplineID)
        {
            bool successful = true;

            try
            {
                Discipline discipline = await _db.Disciplines.Where(x => !x.IsArchived).Include(x => x.Enrollments).SingleAsync(x => x.DisciplineID == disciplineID);

                IEnumerable<int> studentEnrolled = discipline.Enrollments.Where(x => x.IsArchived == false).Select(x => x.StudentID);

                List<Student> availableStudents = await _db.Students.Where(x => x.IsArchived == false && x.FirstName != "Admin" && !studentEnrolled.Contains(x.StudentID)).ToListAsync();

                var studentSelectList = new List<SelectListItem>();

                if (availableStudents.Count == 0)
                {
                    return false;
                }
                else
                {
                    foreach (Student student in availableStudents)
                    {
                        studentSelectList.Add(
                            new SelectListItem { Text = $"{student.FirstName} {student.LastName}", Value = student.StudentID.ToString() }
                        );
                    }
                }

                ViewBag.Students = studentSelectList;
            }
            catch (Exception)
            {
                successful = false;
            }


            return successful;
        }

        // Generates ViewBag.Disciplines with discplines that are available for a student to enroll in.
        private async Task<bool> PopulateDisciplinesAvailableForStudent(int studentID)
        {
            bool successful = true;

            try
            {
                Student student = await _db.Students.Where(x => !x.IsArchived).Include(x => x.Enrollments).SingleAsync(x => x.StudentID == studentID);

                IEnumerable<int> studentEnrolled = student.Enrollments.Where(x => !x.IsArchived).Select(x => x.DisciplineID);

                List<Discipline> availableDisciplines = await _db.Disciplines.Where(x => x.IsArchived == false && x.Name != SystemConstants.Walkin && !studentEnrolled.Contains(x.DisciplineID)).ToListAsync();

                var disciplineSelectList = new List<SelectListItem>();

                if (availableDisciplines.Any())
                {
                    foreach (Discipline disc in availableDisciplines)
                    {
                        disciplineSelectList.Add(
                            new SelectListItem { Text = $"{disc.Name} - ${disc.DefaultCostPerMonth} / Month", Value = disc.DisciplineID.ToString() }
                        );
                    }
                }

                AppSetting appSetting = await _db.AppSettings.Where(x => x.AppSettingID > 0).FirstOrDefaultAsync();
                disciplineSelectList.Add(
                    new SelectListItem { Text = $"Gym Package - ${appSetting.BundleCostPerMonth} / Month", Value = "-1" }
                );
                disciplineSelectList.Add(
                    new SelectListItem { Text = $"Gym Package - ${appSetting.BundleCostPerThreeMonths} / 3-Month", Value = "-2" }
                );
                disciplineSelectList.Add(
                    new SelectListItem { Text = $"Gym Package - ${appSetting.BundleCostPerYear} / Year", Value = "-3" }
                );

                ViewBag.Disciplines = disciplineSelectList;
            }
            catch (Exception)
            {
                successful = false;
            }

            return successful;
        }
    }
}
