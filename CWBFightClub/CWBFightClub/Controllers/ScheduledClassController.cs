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
    public class ScheduledClassController : BaseController
    {
        public ScheduledClassController(CWBContext db, IAccessChecker ac)
            : base(ac, db)
        {
        }

        // Save the scheduled class array from javascript to the database.
        // Without hooking into events it compares the array from JS and the contents of the db and adjusts as necessary.
        public async Task<IActionResult> Save(List<JsCalendarClass> classData)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            List<ScheduledClass> scheduledClasses = await _db.ScheduledClasses
                .Where(x => !x.IsArchived && x.Name != SystemConstants.Walkin)
                .ToListAsync();
            List<int> scheduledClassIDsInDB = scheduledClasses.Select(x => x.ScheduledClassID).ToList();

            List<int> scheduledClassIDsFromJS = new List<int>();
            
            foreach (JsCalendarClass jsCal in classData)
            {
                int validID;
                bool successfulParse = Int32.TryParse(jsCal.id, out validID);
                if (successfulParse && scheduledClassIDsInDB.Contains(validID))
                {
                    //Update here.
                    bool success = await Update(jsCal);
                    //Add to list for delete compare.
                    scheduledClassIDsFromJS.Add(validID);
                }
                else
                {
                    //Add here.
                    await Add(jsCal);
                }
            }

            // Delete check.
            foreach (int dbID in scheduledClassIDsInDB)
            {
                if (!scheduledClassIDsFromJS.Contains(dbID))
                {
                    //Delete
                    bool success = await Delete(dbID);
                    
                    if (!success)
                    {
                        return RedirectToAction("ObjectNotFound", new { type = typeof(ScheduledClass).Name, message = "Deletion failed." });
                    }                    
                }
            }

            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Add a scheduled class from the calendar.
        public async Task Add(JsCalendarClass classData)
        {
            ScheduledClass scheduledClass = new ScheduledClass()
            {
                DisciplineID = int.Parse(classData.calendarId),
                Name = classData.title,
                Start = DateTime.Parse(classData.start),
                End = DateTime.Parse(classData.end),
                HasRecurrence = classData.hasRecurrence,
                RecurrenceFrequency = classData.recurrenceFrequency,
                RecurrenceTime = classData.recurrenceTime
            };

            AssignCreator(scheduledClass);
            await _db.ScheduledClasses.AddAsync(scheduledClass);
        }

        // Generates viewbag data to be used by the calendar for available disciplines.
        private void SetDisciplinesAvailable()
        {
            List<Discipline> disciplines;
            disciplines = _db.Disciplines
                .Where(x => !x.IsArchived && x.Name != SystemConstants.Walkin)
                .OrderBy(x => x.DisciplineID)
                .ToList();

            List<int> discIDs = new List<int>();
            List<string> discNames = new List<string>();
            List<string> discColors = new List<string>();

            foreach (Discipline d in disciplines)
            {
                discIDs.Add(d.DisciplineID);
                discNames.Add(d.Name);
                discColors.Add(d.CalendarColor);
            }

            ViewBag.DiscIDs = discIDs;
            ViewBag.DiscNames = discNames;
            ViewBag.Colors = discColors;
        }

        // Depricated standard create method.
        public IActionResult Create()
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            PopulateDisciplines();

            return View();
        }

        // Depricated standard create post method.
        [HttpPost]
        public IActionResult Create(ScheduledClass scheduledClass)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (scheduledClass is null)
            {
                return Error();
            }
                
            if (!ModelState.IsValid)
            {
                PopulateDisciplines();
                return View(scheduledClass);
            }

            AssignCreator(scheduledClass);

            _db.ScheduledClasses.Add(scheduledClass);
            _db.SaveChanges();
            

            return RedirectToAction(nameof(Index));
        }

        // Depricated standard delete method.
        public async Task<bool> Delete(int id)
        {

            ScheduledClass scheduledClass = await _db.FindAsync<ScheduledClass>(id);

            if (scheduledClass is null || scheduledClass.IsArchived == true)
            {
                return false;
            }

            // Archive related attendance records if they exist.
            List<AttendanceRecord> attendanceRecords = await _db.AttendanceRecords
                .Where(x => x.IsArchived == false && x.StudentID == scheduledClass.ScheduledClassID).ToListAsync();
            foreach (AttendanceRecord ar in attendanceRecords)
            {
                ar.IsArchived = true;
                _db.AttendanceRecords.Update(ar);
            }

            AssignModifier(scheduledClass);
            scheduledClass.IsArchived = true;
            _db.ScheduledClasses.Update(scheduledClass);

            return true;
        }

        // Depricated standard update method.
        public async Task<bool> Update(JsCalendarClass jsData)
        {

            ScheduledClass scheduledClass = await _db.ScheduledClasses.FindAsync(int.Parse(jsData.id));

            if (scheduledClass is null || scheduledClass.IsArchived == true)
            {
                return false;
            }

            // Logic for converting js calendar to scheduled class.
            scheduledClass.DisciplineID = int.Parse(jsData.calendarId);
            scheduledClass.Name = jsData.title;
            scheduledClass.Start = DateTime.Parse(jsData.start);
            scheduledClass.End = DateTime.Parse(jsData.end);
            scheduledClass.HasRecurrence = jsData.hasRecurrence;
            scheduledClass.RecurrenceFrequency = jsData.recurrenceFrequency;
            scheduledClass.RecurrenceTime = jsData.recurrenceTime;

            //AssignModifier(scheduledClass);

            _db.ScheduledClasses.Update(scheduledClass);

            return true;
        }

        // Display the calendar for scheduled classes.
        public async Task<IActionResult> Index()
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            IEnumerable<ScheduledClass> scheduledClasses = null;

            await Task.Run(() =>
            {
                scheduledClasses = _db.ScheduledClasses
                                     .Where(x => x.IsArchived == false && x.Name != SystemConstants.Walkin)
                                     .Include(nameof(ScheduledClass.Discipline))
                                     .AsNoTracking();
            });

            SetDisciplinesAvailable();

            return View(scheduledClasses);
        }

        // Depricated select list for disciplines.
        private void PopulateDisciplines()
        {
            var disciplines = _db.Disciplines.Where(x => !x.IsArchived).ToList();
            var discSelectList = new List<SelectListItem>();

            foreach (var disc in disciplines)
            {
                discSelectList.Add(
                    new SelectListItem { Text = disc.Name, Value = disc.DisciplineID.ToString() }
                );
            }

            ViewBag.DisciplineID = discSelectList;
        }
    }

    // Class used to retrieve data from the front end.
    public class JsCalendarClass
    {
        public string id { get; set; } //Scheduled Class ID
        public string calendarId { get; set; } //Disc ID
        public string title { get; set; } // Title.
        public string start { get; set; }
        public string end { get; set; }
        public bool hasRecurrence { get; set; }
        public string recurrenceFrequency { get; set; }
        public int recurrenceTime { get; set; }
    }
}
