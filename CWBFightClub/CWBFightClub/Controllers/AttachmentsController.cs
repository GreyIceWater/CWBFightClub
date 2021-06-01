using CWBFightClub.Data;
using CWBFightClub.Models;
using CWBFightClub.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWBFightClub.Controllers
{
    public class AttachmentsController : BaseController
    {
        private readonly IStudentUtility _studentUtil;

        private readonly CWBContext _context;


        public AttachmentsController(CWBContext db, IStudentUtility studentUtil, IAccessChecker ac) : base(ac, db)
        {
            _studentUtil = studentUtil;
            _context = db;
        }

        /// <summary>
        /// Gets the student and includes the list of attachments.
        /// </summary>
        /// <param name="studentid">The student to find.</param>
        /// <returns>The View with all student attachments for the selected student.</returns>
        public async Task<IActionResult> Index(int studentid)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            ViewBag.studentid = studentid;
            Student student = await FindStudent(studentid);

            ViewBag.studentname = student.FirstName + " " + student.LastName;
            ViewBag.studentid = student.StudentID;

            ViewBag.Model = student;

            Student student1 = await _db.Students.Include(s => s.FilePaths).SingleOrDefaultAsync(s => s.StudentID == studentid);
            if (student1 == null)
            {
                return null;
            }

            return View(student1);
        }

        // Generates Upload Error screen for error handling.
        public async Task<IActionResult> UploadError(int studentid)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            ViewBag.studentid = studentid;
            Student student = await FindStudent(studentid);

            ViewBag.studentname = student.FirstName + " " + student.LastName;
            ViewBag.studentid = student.StudentID;

            ViewBag.Model = student;

            Student student1 = await _db.Students.Include(s => s.FilePaths).SingleOrDefaultAsync(s => s.StudentID == studentid);
            if (student1 == null)
            {
                return null;
            }

            return View(student1);
        }

        // Get method for retrieving edit view.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filePath = await _context.FilePaths.FindAsync(id);
            if (filePath == null)
            {
                return NotFound();
            }

            ViewData["StudentID"] = new SelectList(_context.Students, "StudentID", "FirstName", filePath.StudentID);
            return View(filePath);
        }

        /// <summary>
        /// Gets a student by ID.
        /// </summary>
        /// <param name="id">The student ID to search with.</param>
        /// <returns>The found student.</returns>
        private async Task<Student> FindStudent(int id)
        {
            var student = await _db.Students.Where(x => x.StudentID == id && x.IsArchived == false).FirstOrDefaultAsync();

            return student;
        }
    }
}
