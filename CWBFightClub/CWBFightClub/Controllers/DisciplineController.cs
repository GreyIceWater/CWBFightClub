using CWBFightClub.Data;
using CWBFightClub.Models;
using CWBFightClub.Services;
using CWBFightClub.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWBFightClub.Controllers
{
    public class DisciplineController : BaseController
    {
        private readonly ILogger<DisciplineController> _logger;

        public DisciplineController(ILogger<DisciplineController> logger, CWBContext db, IAccessChecker ac) : base(ac, db)
        {
            _logger = logger;
        }

        // Standard Get method for create.
        public IActionResult Create()
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            return View();
        }

        // Standard Post method for create.
        [HttpPost]
        public IActionResult Create(Discipline discipline)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            try
            {
                if (discipline is not null)
                {
                    if (!ModelState.IsValid)
                    {
                        return View(discipline);
                    }

                    AssignCreator(discipline);

                    _db.Disciplines.Add(discipline);
                    _db.SaveChanges();
                }
                else
                {
                    return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Discipline).Name, message = "Create Discipline Failed." });
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return RedirectToAction("Index");
        }

        // Standard Get method for delete. id = discipline ID.
        public async Task<IActionResult> Delete(int? id)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (id == null)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Discipline).Name, message = "Delete failed." });
            }

            Discipline discipline;

            try
            {
                discipline = await _db.FindAsync<Discipline>(id);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            if (discipline == null || discipline.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Discipline).Name, message = "Delete failed." });
            }

            try
            {
                await _db.Entry(discipline).Collection(x => x.Belts).LoadAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return View(discipline);
        }

        // Standard Post method for delete.
        [HttpPost]
        public async Task<IActionResult> Delete(Discipline discipline)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (discipline is not null && discipline.IsArchived is false)
            {

                try
                {
                    discipline = await _db.Disciplines.Include(x => x.Belts).SingleAsync(x => x.DisciplineID == discipline.DisciplineID);

                    if (discipline.Belts != null)
                    {
                        foreach (Belt b in discipline.Belts)
                        {
                            b.IsArchived = true;
                            AssignModifier(b);
                            _db.Belts.Update(b);
                        }
                    }

                    AssignModifier(discipline);
                    discipline.IsArchived = true;
                    _db.Disciplines.Update(discipline);


                    // Archive related enrollments if they exist.
                    List<Enrollment> enrollments = await _db.Enrollments
                        .Where(x => x.IsArchived == false && x.DisciplineID == discipline.DisciplineID)
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

                    // Archive related scheduled classes and attendance records.
                    List<ScheduledClass> scheduledClasses = await _db.ScheduledClasses
                        .Where(x => !x.IsArchived && x.DisciplineID == discipline.DisciplineID)
                        .Include(x => x.AttendanceRecords.Where(x => !x.IsArchived)).ToListAsync();

                    foreach (ScheduledClass s in scheduledClasses)
                    {
                        s.IsArchived = true;
                        _db.ScheduledClasses.Update(s);

                        foreach (AttendanceRecord ar in s.AttendanceRecords)
                        {
                            ar.IsArchived = true;
                            _db.AttendanceRecords.Update(ar);
                        }
                    }

                    await _db.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }

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
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Discipline).Name, message = "Edit failed." });
            }

            Discipline discipline;
            try
            {
                discipline = await _db.Disciplines.FindAsync(id);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            if (discipline == null || discipline.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Discipline).Name, message = "Edit failed." });
            }

            List<Belt> allBelts;
            try
            {
                allBelts = _db.Belts.Where(x => x.DisciplineID == discipline.DisciplineID && x.IsArchived == false).OrderBy(x => x.Rank).ToList();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            foreach (Belt b in allBelts)
            {
                if (b.DisciplineID == discipline.DisciplineID)
                {
                    discipline.Belts.Add(b);
                }
            }

            return View(discipline);
        }

        // Standard Post method for edit.
        [HttpPost]
        public async Task<IActionResult> Edit(Discipline discipline)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (discipline != null && discipline.IsArchived == false)
            {
                if (!ModelState.IsValid)
                {
                    return View(discipline);
                }

                AssignModifier(discipline);

                try
                {
                    _db.Disciplines.Update(discipline);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }

            return RedirectToAction("Index");
        }

        // Lists disciplines in db.
        public async Task<IActionResult> Index()
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            IEnumerable<Discipline> disciplines = null;

            try
            {
                await Task.Run(() =>
                {
                    disciplines = _db.Disciplines.Where(x => x.IsArchived == false && x.Name != SystemConstants.Walkin);
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return View(disciplines);
        }
    }
}
