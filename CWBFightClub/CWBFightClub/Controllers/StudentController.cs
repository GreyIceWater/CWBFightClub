using CWBFightClub.Data;
using CWBFightClub.Models;
using CWBFightClub.Services;
using CWBFightClub.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CWBFightClub.Controllers
{
    public class StudentController : BaseController
    {
        private readonly IStudentUtility _studentUtil;

        private IWebHostEnvironment _webHost;

        public StudentController(CWBContext db, IStudentUtility studentUtil, IAccessChecker ac, IWebHostEnvironment webhost) : base(ac, db)
        {
            _studentUtil = studentUtil;
            _webHost = webhost;
        }

        public IActionResult Create()
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            // Set viewbag bool based on whether a user is logged in to let the view know.
            ViewBag.noLogin = checkResult == null ? false : true;

            return View();
        }

        [HttpPost]
        public IActionResult Create(Student student)
        {
            // Set variable to determine if user is logged in.
            IActionResult checkResult = this.AccessChecker.CheckForAccess();

            ViewBag.noLogin = checkResult == null ? false : true;

            if (student is not null)
            {
                if (!string.IsNullOrWhiteSpace(student.Phone))
                {
                    student.Phone = _studentUtil.ConvertNumberToMobileFormat(student.Phone);
                }

                if (!ModelState.IsValid)
                {
                    return View(student);
                }

                AssignCreator(student);

                _db.Students.Add(student);
                _db.SaveChanges();
            }

            Guid guid = Guid.NewGuid();
            TempData["SecurityCheck"] = guid;
            return RedirectToAction("Student", "Enrollment", new { id = student.StudentID, fromStudentWorkflow = true });
        }

        // Creates a file for the student attachments area.
        [HttpPost]
        public IActionResult CreateFile(Student student, IFormFile upload, string comment, string filename, FileType type)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }


            try
            {
                if (upload != null && upload.Length > 0)
                {
                    if (upload.FileName != filename && filename != null)
                    {
                        // get extension
                        string ext = Path.GetExtension(upload.FileName);

                        // check filetypes
                        var supportedTypes = new[] { ".png", ".jpg", ".pdf", ".gif", ".bmp", ".raw", ".jpeg", ".JPG" };
                        if (!supportedTypes.Contains(ext))
                        {
                            return Redirect("~/Attachments/UploadError?studentid=" + student.StudentID);
                        }


                        filename = filename + ext;

                        // check if file already exists
                        string pathconcat = "wwwroot/Images/" + filename;
                        var checkfile = System.IO.File.Exists(pathconcat);

                        if (checkfile == true)
                        {
                            return Redirect("~/Attachments/UploadError?studentid=" + student.StudentID);
                        }
                        else
                        {
                            var photo = new FilePath
                            {
                                FileName = System.IO.Path.GetFileName(filename),
                                FileType = type,
                                StudentID = student.StudentID,
                                Comment = comment,
                                DateCreated = DateTime.Now,
                                //Student = student
                            };

                            //student.FilePaths = new List<FilePath>();
                            //student.FilePaths.Add(photo);

                            string folderpath = Path.Combine(_webHost.WebRootPath, "Images");
                            folderpath = Path.Combine(folderpath, filename);

                            using (var fileStream = new FileStream(folderpath, FileMode.Create)) { upload.CopyTo(fileStream); }
                            _db.FilePaths.Add(photo);
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        // check filetypes
                        string ext = Path.GetExtension(upload.FileName);

                        var supportedTypes = new[] { ".png", ".jpg", ".pdf", ".gif", ".bmp", ".raw", ".jpeg", ".JPG" };
                        if (!supportedTypes.Contains(ext))
                        {
                            return Redirect("~/Attachments/UploadError?studentid=" + student.StudentID);
                        }

                        // check if file already exists
                        string pathconcat = "wwwroot/Images/" + upload.FileName;
                        var checkfile = System.IO.File.Exists(pathconcat);

                        if (checkfile == true)
                        {
                            return Redirect("~/Attachments/UploadError?studentid=" + student.StudentID);
                        }
                        else
                        {
                            var photo = new FilePath
                            {
                                FileName = System.IO.Path.GetFileName(upload.FileName),
                                FileType = type,
                                StudentID = student.StudentID,
                                Comment = comment,
                                DateCreated = DateTime.Now,
                                //Student = student
                            };

                            //student.FilePaths = new List<FilePath>();
                            //student.FilePaths.Add(photo);

                            string folderpath = Path.Combine(_webHost.WebRootPath, "Images");
                            folderpath = Path.Combine(folderpath, upload.FileName);

                            using (var fileStream = new FileStream(folderpath, FileMode.Create)) { upload.CopyTo(fileStream); }
                            _db.FilePaths.Add(photo);
                            _db.SaveChanges();
                        }
                    }

                }

                return Redirect("~/Attachments/Index?studentid=" + student.StudentID);

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }

            return Redirect("~/Attachments/Index?studentid=" + student.StudentID);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (id is null)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Student).Name, message = "Deletion failed." });
            }

            Student student = await _db.FindAsync<Student>(id);

            if (student is null || student.IsArchived is true)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Student).Name, message = "Deletion failed." });
            }

            return View(student);
        }

        // Archives a student. Cascade archives related entities.
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePost(Student student)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (student is null)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Student).Name, message = "Deletion failed." });
            }

            Student studentFound = await _db.FindAsync<Student>(student.StudentID);

            if (studentFound is null || studentFound.IsArchived is true)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Student).Name, message = "Deletion failed." });
            }

            AssignModifier(studentFound);
            studentFound.IsArchived = true;

            // Archive related account if it exists.
            await Task.Run(() =>
            {
                Account account = _db.Accounts.Where(x => x.IsArchived == false && x.StudentID == studentFound.StudentID)
                               .FirstOrDefault();
                if (account is not null)
                {
                    account.IsArchived = true;
                    _db.Accounts.Update(account);
                }
            });

            // Archive related payments if they exist.
            List<Payment> payments = await _db.Payments.Where(x => x.IsArchived == false && x.StudentID == studentFound.StudentID).ToListAsync();
            foreach (Payment p in payments)
            {
                p.IsArchived = true;
                _db.Payments.Update(p);
            }

            // Archive related enrollments if they exist.
            List<Enrollment> enrollments = await _db.Enrollments
                .Where(x => x.IsArchived == false && x.StudentID == studentFound.StudentID)
                .Include(x => x.AchievedBelts.Where(x => x.IsArchived == false)).ToListAsync();
            foreach (Enrollment e in enrollments)
            {
                e.IsArchived = true;
                _db.Enrollments.Update(e);

                foreach (AchievedBelt a in e.AchievedBelts)
                {
                    a.IsArchived = true;
                    _db.AchievedBelts.Update(a);
                }
            }

            // Archive related attendance records if they exist.
            List<AttendanceRecord> attendanceRecords = await _db.AttendanceRecords
                .Where(x => x.IsArchived == false && x.StudentID == studentFound.StudentID).ToListAsync();
            foreach (AttendanceRecord ar in attendanceRecords)
            {
                ar.IsArchived = true;
                _db.AttendanceRecords.Update(ar);
            }

            // Archive related StudentGuardian records if they exist.
            List<StudentGuardian> studentGuardians = await _db.StudentGuardians
                .Where(x => x.IsArchived == false && x.StudentID == studentFound.StudentID).ToListAsync();

            List<int> guardianIds = new();

            foreach(StudentGuardian sg in studentGuardians)
            {
                guardianIds.Add(sg.GuardianID);
                sg.IsArchived = true;
                AssignModifier(sg);
                _db.StudentGuardians.Update(sg);                
            }

            _db.Students.Update(studentFound);
            _db.SaveChanges();

            // Archive any guardian records if applicable.
            await CheckForStudentGuardianRecords(guardianIds);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (id is null)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Student).Name, message = "Edit failed." });
            }

            Student student = await _db.FindAsync<Student>(id);

            if (student is null || student.IsArchived is true)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Student).Name, message = "Edit failed." });
            }

            student = await _db.Students.Include(x => x.StudentGuardians.Where(x => x.IsArchived == false)).ThenInclude(x => x.Guardian).SingleAsync(x => x.StudentID == id);

            return View(student);
        }

        // Gets attendance records for the student.
        public async Task<IActionResult> Attendance(int studentId, string sortOrder = null, string searchString = null, int? pageNumber = 1)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }
            if (studentId == 0)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = "Student not found", message = "Attenance records failed." });
            }

            Student student = _db.Students.Where(x => x.IsArchived == false && x.StudentID == studentId).FirstOrDefault();
            
            IEnumerable<AttendanceRecord> records = null;
            await Task.Run(() =>
            {
                records = _db.AttendanceRecords
                                        .Where(x => x.IsArchived == false && x.StudentID == studentId)
                                        .Include(x => x.ScheduledClass)
                                        .ThenInclude(scheduledClass => scheduledClass.Discipline)
                                        .Include(x => x.Student)
                                        .OrderBy(s => s.Start)
                                        .AsNoTracking();
            });

            ViewBag.studentid = studentId;
            ViewBag.studentname = student.FirstName + " " + student.LastName;

            return View(records);
        }

        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditPost(Student student)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (student == null)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Student).Name, message = "Edit failed." });
            }

            var studentToUpdate = await _db.FindAsync<Student>(student.StudentID);

            if (studentToUpdate is null || studentToUpdate.IsArchived is true)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Student).Name, message = "Edit failed." });
            }

            if (!ModelState.IsValid)
            {
                return View(studentToUpdate);
            }

            if (await TryUpdateModelAsync<Student>(studentToUpdate, "",
                x => x.FirstName,
                x => x.MiddleName,
                x => x.LastName,
                x => x.DOB,
                x => x.StreetAddress,
                x => x.City,
                x => x.State,
                x => x.ZIP,
                x => x.Phone,
                x => x.Email,
                x => x.WaiverSigned,
                x => x.IsInstructor))
            {

                if (!string.IsNullOrWhiteSpace(studentToUpdate.Phone))
                {
                    studentToUpdate.Phone = _studentUtil.ConvertNumberToMobileFormat(studentToUpdate.Phone);
                }

                AssignModifier(studentToUpdate);

                _db.Students.Update(studentToUpdate);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(studentToUpdate);
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber = 1)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();

            if (checkResult != null)
            {
                return checkResult;
            }

            IEnumerable<Student> students = null;

            await Task.Run(() =>
            {
                students = _db.Students.Where(x => x.IsArchived == false && x.FirstName != "Admin")
                               .AsNoTracking();
            });

            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentSearch"] = searchString;

            ViewData["FirstNameSortParm"] = sortOrder == "FirstName" ? "firstname_desc" : "FirstName";
            ViewData["LastNameSortParm"] = sortOrder == "LastName" ? "lastname_desc" : "LastName";
            ViewData["DOBSortParm"] = sortOrder == "DOB" ? "dob_desc" : "DOB";
            ViewData["PhoneSortParm"] = sortOrder == "Phone" ? "phone_desc" : "Phone";
            ViewData["EmailSortParm"] = sortOrder == "Email" ? "email_desc" : "Email";

            switch (sortOrder)
            {
                case "FirstName":
                    students = students.OrderBy(s => s.FirstName);
                    break;
                case "firstname_desc":
                    students = students.OrderByDescending(s => s.FirstName);
                    break;
                case "LastName":
                    students = students.OrderBy(s => s.LastName);
                    break;
                case "lastname_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "DOB":
                    students = students.OrderBy(s => s.DOB);
                    break;
                case "dob_desc":
                    students = students.OrderByDescending(s => s.DOB);
                    break;
                case "Phone":
                    students = students.OrderBy(s => s.Phone);
                    break;
                case "phone_desc":
                    students = students.OrderByDescending(s => s.Phone);
                    break;
                case "Email":
                    students = students.OrderBy(s => s.Email);
                    break;
                case "email_desc":
                    students = students.OrderByDescending(s => s.Email);
                    break;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.FirstName.ToLower().Contains(searchString.ToLower()) ||
                            s.LastName.ToLower().Contains(searchString.ToLower()));
            }

            return View(PaginatedList<Student>.Create(students.AsQueryable(), pageNumber.Value, SystemConstants.ItemsPerPage));
        }
    }
}
