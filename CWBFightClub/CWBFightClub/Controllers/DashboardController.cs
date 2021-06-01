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
    public class DashboardController : BaseController
    {
        private IStudentUtility _studentUtility;
        public DashboardController(IAccessChecker ac, CWBContext db, IStudentUtility studentUtility) : base(ac, db)
        {
            _studentUtility = studentUtility;
        }

        // Dashboard landing page for administrative utilities and reports.
        public IActionResult Index()
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            return View();
        }

        // Global app settings Get method.
        public IActionResult Settings()
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            AppSetting appSetting = _db.AppSettings.Where(x => x.AppSettingID > 0).FirstOrDefault();

            return View(appSetting);
        }

        // Post method for global app settings.
        [HttpPost]
        public async Task<IActionResult> Settings(AppSetting appSetting)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            AppSetting appSettingtoUpdate = await _db.FindAsync<AppSetting>(appSetting.AppSettingID);

            if (!await TryUpdateModelAsync<AppSetting>(appSettingtoUpdate, "",
                x => x.BundleCostPerMonth,
                x => x.BundleCostPerThreeMonths,
                x => x.BundleCostPerYear,
                x => x.PercentOfClassRequiredToVerify))
            {
                return View(appSetting);
            }

            ViewBag.Saved = true;
            _db.AppSettings.Update(appSettingtoUpdate);
            _db.SaveChanges();
            return View(appSetting);
        }

        // Reports landing page.
        public IActionResult Reports()
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            return View();
        }

        // Attendance report Get method.
        public IActionResult Attendance()
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            return View(new StudentAttendanceReport());
        }

        /// <summary>
        /// Generates report data for students that have no attendance records or were created
        /// a number of days prior to integer chosen by user input.
        /// </summary>
        /// <param name="attendanceReport">The attendance report to construct.</param>
        [HttpPost]
        public async Task<IActionResult> Attendance(StudentAttendanceReport attendanceReport)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            List<Student> students;

            if (attendanceReport.IncludeInstructors)
            {
                students = await _db.Students.Where(x => !x.IsArchived && x.FirstName != "Admin")
                .Include(x => x.AttendanceRecords.Where(x => !x.IsArchived))
                .ThenInclude(x => x.ScheduledClass)
                .ThenInclude(x => x.Discipline)
                .Include(x => x.Enrollments.Where(x => !x.IsArchived && (x.EndDate == null || x.EndDate <= DateTime.Now)))
                .ToListAsync();
            }
            else
            {
                students = await _db.Students.Where(x => !x.IsArchived && x.IsInstructor == false)
                .Include(x => x.AttendanceRecords.Where(x => !x.IsArchived))
                .ThenInclude(x => x.ScheduledClass)
                .ThenInclude(x => x.Discipline)
                .Include(x => x.Enrollments.Where(x => !x.IsArchived && (x.EndDate == null || x.EndDate <= DateTime.Now)))
                .ToListAsync();
            }

            List<string> studentNames = new List<string>();
            List<DateTime> dateOfLastAttendanceEntry = new List<DateTime>(); // Can derive days from this.
            List<int> daysSinceLastAttendanceEntry = new List<int>();
            List<string> lastDiscAttended = new List<string>();
            List<int> studentIDs = new List<int>();
            List<string> studentPhones = new List<string>();

            List<Student> studentListToSearch;
            if (attendanceReport.IncludeOnlyActiveEnrollments)
            {
                studentListToSearch = students.Where(x => x.Enrollments.Any(x => !x.EndDate.HasValue || x.EndDate >= DateTime.Now)).ToList();
            }
            else
            {
                studentListToSearch = students;
            }

            foreach (Student s in studentListToSearch.OrderBy(x => x.CreatedDate))
            {
                int days = (DateTime.Now - s.CreatedDate).Days; //Debug

                if (s.AttendanceRecords.Any())
                {
                    AttendanceRecord lastAttendance = s.AttendanceRecords.OrderByDescending(x => x.Start).FirstOrDefault();
                    days = (DateTime.Now - lastAttendance.Start).Days; //dubug
                    if ((DateTime.Now - lastAttendance.Start).Days >= attendanceReport.DaysSinceLastAttendanceRecord)
                    {
                        studentNames.Add("'" + s.FirstName + " " + s.LastName + "'");
                        studentIDs.Add(s.StudentID);
                        studentPhones.Add(s.Phone);
                        dateOfLastAttendanceEntry.Add(lastAttendance.Start);
                        daysSinceLastAttendanceEntry.Add((DateTime.Now - lastAttendance.Start).Days);
                        lastDiscAttended.Add(lastAttendance.ScheduledClass.Discipline.Name);
                    }
                }
                else if ((DateTime.Now - s.CreatedDate).Days >= attendanceReport.DaysSinceLastAttendanceRecord)
                {
                    studentNames.Add("'" + s.FirstName + " " + s.LastName + "'");
                    studentIDs.Add(s.StudentID);
                    studentPhones.Add(s.Phone);
                    dateOfLastAttendanceEntry.Add(s.CreatedDate);
                    daysSinceLastAttendanceEntry.Add((DateTime.Now - s.CreatedDate).Days); 
                    lastDiscAttended.Add(null); //Use for determining attendance records exist.
                }
            }

            for (int i = 0; i < studentIDs.Count; i++)
            {
                attendanceReport.Results.Add(new StudentAttendanceReportResult
                {
                    TableStudent = studentNames[i],
                    StudentID = studentIDs[i],
                    StudentPhone = studentPhones[i],
                    TableDaysSinceLastAttendance = daysSinceLastAttendanceEntry[i],
                    DateOfLastAttendance = dateOfLastAttendanceEntry[i],
                    LastDisciplineAttended = lastDiscAttended[i]
                });
            }

            attendanceReport.Results = attendanceReport.Results.OrderByDescending(x => x.TableDaysSinceLastAttendance).ToList();

            attendanceReport.GraphLabels = string.Join(", ", attendanceReport.Results.Select(x => x.TableStudent).Take(7));
            attendanceReport.GraphDatas = string.Join(", ", attendanceReport.Results.Select(x => x.TableDaysSinceLastAttendance).Take(7));

            ViewBag.FromPost = true;

            return View(attendanceReport);
        }

        // Display page and prompt for student progress report.
        public IActionResult StudentProgressClasses()
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            IEnumerable<Student> students = _db.Students.Where(x => !x.IsArchived)
                .Include(x => x.AttendanceRecords.Where(x => !x.IsArchived && x.IsVerified == false))
                .ThenInclude(x => x.ScheduledClass);

            _studentUtility.VerifyAttendanceRecord(students, _db);

            PopulateDisciplines();

            return View(new StudentProgressReportClasses());
        }

        // Generates and displays student progress report by class count.
        [HttpPost]
        public IActionResult StudentProgressClasses(StudentProgressReportClasses progressReport)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            progressReport.Discipline = _db.Disciplines.Where(x => x.DisciplineID == progressReport.DisciplineID).Select(x => x.Name).FirstOrDefault();

            Dictionary<int, string> studentIDtoName = new Dictionary<int, string>();
            Dictionary<int, double> labelData = new Dictionary<int, double>();
            Dictionary<int, DateTime> studentIDtoDateOfLastRank = new Dictionary<int, DateTime>();

            List<Student> students = _db.Students.Where(x => !x.IsArchived && x.FirstName != "Admin")
                .Include(x => x.AttendanceRecords.Where(x => !x.IsArchived && x.IsVerified == true))
                .ThenInclude(x => x.ScheduledClass)
                .Include(x => x.Enrollments.Where(x => !x.IsArchived))
                .ThenInclude(x => x.AchievedBelts.Where(x => !x.IsArchived)).ToList();

            List<Student> enrolledStudents = students.Where(x => x.Enrollments.Select(x => x.DisciplineID).Contains(progressReport.DisciplineID)).ToList();

            foreach (Student s in enrolledStudents)
            {
                DateTime dateOfLastRank = s.Enrollments.Where(x => !x.IsArchived && x.DisciplineID == progressReport.DisciplineID)
                    .FirstOrDefault()
                    .AchievedBelts.Where(x => !x.IsArchived)
                    .OrderByDescending(x => x.DateAchieved)
                    .Select(x => x.DateAchieved).FirstOrDefault();

                int classCountPastCurrentRank = s.AttendanceRecords
                    .Where(x => x.ScheduledClass.DisciplineID == progressReport.DisciplineID && x.Start > dateOfLastRank && x.IsVerified).Count();
                if (classCountPastCurrentRank >= progressReport.StudentClassesPastCurrentRank)
                {
                    progressReport.Results.Add(new StudentProgressReportClassesResult
                    {
                        ClassCount = classCountPastCurrentRank,
                        DateOfLastRank = dateOfLastRank,
                        Student = $"'{s.FirstName} {s.LastName}'",
                        StudentID =  s.StudentID
                    });
                }
            }

            progressReport.Results = progressReport.Results.OrderByDescending(x => x.ClassCount).ToList();

            progressReport.GraphLabels = string.Join(", ", progressReport.Results.Select(x => x.Student).Take(7));
            progressReport.GraphDatas = string.Join(", ", progressReport.Results.Select(x => x.ClassCount).Take(7));

            ViewBag.FromPost = true;

            PopulateDisciplines();

            return View(progressReport);
        }

        // Prompts for inputs for depricated report for student progress by hours.
        public IActionResult StudentProgress()
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            PopulateDisciplines();

            return View(new StudentProgressReport());
        }

        // Generates depricated report for student progress by hours.
        [HttpPost]
        public IActionResult StudentProgress(StudentProgressReport progressReport)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            PopulateDisciplines();

            if (progressReport.DisciplineID == 0)
            {
                return View();
            }

            progressReport.Discipline = _db.Disciplines.Where(x => x.DisciplineID == progressReport.DisciplineID).Select(x => x.Name).FirstOrDefault();

            Dictionary<int, string> studentIDtoName = new Dictionary<int, string>();
            Dictionary<int, double> labelData = new Dictionary<int, double>();
            Dictionary<int, DateTime> studentIDtoDateOfLastRank = new Dictionary<int, DateTime>();

            List<Student> students = _db.Students.Where(x => !x.IsArchived && x.FirstName != "Admin")
                .Include(x => x.AttendanceRecords.Where(x => !x.IsArchived))
                .ThenInclude(x => x.ScheduledClass)
                .Include(x => x.Enrollments.Where(x => !x.IsArchived))
                .ThenInclude(x => x.AchievedBelts.Where(x => !x.IsArchived)).ToList();

            List<Student> enrolledStudents = students.Where(x => x.Enrollments.Select(x => x.DisciplineID).Contains(progressReport.DisciplineID)).ToList();
            foreach (Student s in enrolledStudents)
            {
                DateTime dateOfLastRank = s.Enrollments.Where(x => !x.IsArchived && x.DisciplineID == progressReport.DisciplineID)
                    .FirstOrDefault()
                    .AchievedBelts.Where(x => !x.IsArchived)
                    .OrderByDescending(x => x.DateAchieved)
                    .Select(x => x.DateAchieved).FirstOrDefault();

                TimeSpan classHoursPastCurrentRank = s.AttendanceRecords
                    .Where(x => x.ScheduledClass.DisciplineID == progressReport.DisciplineID && x.Start > dateOfLastRank)
                    .Aggregate(TimeSpan.Zero, (sumSoFar, next) => sumSoFar + (next.End - next.Start));
                if (classHoursPastCurrentRank.TotalHours >= (double)progressReport.StudentHoursPastCurrentRank)
                {
                    studentIDtoDateOfLastRank.Add(s.StudentID, dateOfLastRank);
                    studentIDtoName.Add(s.StudentID, $"{s.FirstName} {s.LastName}");
                    labelData.Add(s.StudentID, Math.Round(classHoursPastCurrentRank.TotalHours, 1));
                }
            }

            List<int> studentIDs = new List<int>();
            List<string> labels = new List<string>();
            List<string> datas = new List<string>();
            List<DateTime> dateOfLastRanks = new List<DateTime>();

            foreach (KeyValuePair<int, double> kvp in labelData.OrderByDescending(x => x.Value))
            {
                labels.Add("'" + studentIDtoName[kvp.Key] + "'");
                datas.Add(labelData[kvp.Key].ToString());
                dateOfLastRanks.Add(studentIDtoDateOfLastRank[kvp.Key]);
                studentIDs.Add(kvp.Key);
            }

            progressReport.TableLabels = labels;
            progressReport.TableDatas = datas;
            progressReport.DateOfLastRanks = dateOfLastRanks;
            progressReport.StudentIDs = studentIDs;

            progressReport.GraphLabels = string.Join(", ", labels.Take(7));
            progressReport.GraphDatas = string.Join(", ", datas.Take(7));

            ViewBag.FromPost = true;

            return View(progressReport);
        }

        // Generate balance report for outstanding balances.
        public IActionResult Balance()
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            List<Student> students = _db.Students
                .Where(x => !x.IsArchived && x.FirstName != "Admin" &&
                    x.BalanceDue.HasValue && x.BalanceDue > 0).ToList();

            _studentUtility.UpdateStudentBalance(students, _db);
            StudentBalanceReport balanceReport = new StudentBalanceReport();
            foreach (Student s in students)
            {
                balanceReport.Results.Add(new StudentBalanceReportResult
                {
                    CurrentBalance = s.BalanceDue.Value,
                    StudentID = s.StudentID,
                    StudentPhone = s.Phone,
                    TableStudent = $"{s.FirstName} {s.LastName}"
                });
            }

            balanceReport.Results = balanceReport.Results.OrderByDescending(x => x.CurrentBalance).ToList();

            ViewBag.FromPost = true;

            return View(balanceReport);
        }

        public IActionResult Payment()
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            IEnumerable<Student> students = _db.Students;
            _studentUtility.UpdateStudentBalance(students, _db);

            return View(new StudentPaymentReport());
        }

        // Generates payment report for students that have a payment due in the next number of days.
        [HttpPost]
        public IActionResult Payment(StudentPaymentReport paymentReport)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            List<Student> students = _db.Students
                .Where(x => !x.IsArchived && x.FirstName != "Admin" &&
                    x.BalanceDueDate.HasValue && 
                    x.PaymentAgreementAmount.HasValue &&
                    x.PaymentAgreementAmount.Value > 0m && 
                    x.BalanceDueDate.Value.Date <= DateTime.Now.AddDays(paymentReport.DaysUntilPaymentIsDue).Date).ToList();

            foreach (Student s in students)
            {
                paymentReport.Results.Add(new StudentPaymentReportResult
                {
                    AmountDue = s.PaymentAgreementAmount.Value,
                    CurrentBalance = s.BalanceDue ?? 0m,
                    DueDate = s.BalanceDueDate.Value,
                    StudentID = s.StudentID,
                    StudentPhone = s.Phone,
                    TableStudent = $"'{s.FirstName} {s.LastName}'"
                });
            }

            paymentReport.Results = paymentReport.Results.OrderBy(x => x.DueDate).ToList();

            paymentReport.GraphLabels = string.Join(", ", paymentReport.Results.Select(x => x.TableStudent).Take(7));
            paymentReport.GraphDatas = string.Join(", ", paymentReport.Results.Select(x => x.AmountDue).Take(7));

            ViewBag.FromPost = true;

            return View(paymentReport);
        }

        // Generates select list entries for disciplines for various reports.
        private void PopulateDisciplines()
        {
            var disciplines = _db.Disciplines.Where(x => !x.IsArchived && x.Name != SystemConstants.Walkin).ToList();
            var discSelectList = new List<SelectListItem>();

            foreach (var disc in disciplines)
            {
                discSelectList.Add(
                    new SelectListItem { Text = disc.Name, Value = disc.DisciplineID.ToString() }
                );
            }

            ViewBag.Disciplines = discSelectList;
        }
    }
}
