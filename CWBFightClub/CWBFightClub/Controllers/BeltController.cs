using CWBFightClub.Data;
using CWBFightClub.Models;
using CWBFightClub.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWBFightClub.Controllers
{
    public class BeltController : BaseController
    {
        private readonly ILogger<BeltController> _logger;

        public BeltController(ILogger<BeltController> logger, CWBContext db, IAccessChecker ac) : base(ac, db)
        {
            _logger = logger;
        }

        // Standard Get method for create. id = discipline ID.
        public IActionResult Create(int id)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            Discipline discipline = FindDiscipline(id);

            if (discipline != null)
            {
                ViewBag.DisciplineName = discipline.Name;
            }
            else
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Belt).Name, message = "No discipline found. Please ensure you are adding a belt from a discipline." });
            }

            Belt belt = new()
            {
                DisciplineID = id
            };

            return View(belt);
        }

        // Standard Post method for create.
        [HttpPost]
        public IActionResult Create(Belt belt)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (belt != null)
            {
                if (!ModelState.IsValid)
                {
                    return View(belt);
                }

                try
                {
                    AssignCreator(belt);

                    _db.Belts.Add(belt);
                    _db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            else
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Belt).Name, message = "Create failed." });
            }

            return RedirectToAction("Edit", "Discipline", new { Id = belt.DisciplineID });
        }

        // Standard Get method for delete. id = belt ID.
        public async Task<IActionResult> Delete(int? id)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (id == null)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Belt).Name, message = "Delete failed." });
            }

            Belt belt;
            Discipline discipline;

            try
            {
                belt = await _db.FindAsync<Belt>(id);
                discipline = FindDiscipline(belt.DisciplineID);

            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            if (discipline != null)
            {
                ViewBag.DisciplineName = discipline.Name;
            }
            else
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Belt).Name, message = "No discipline found. Please ensure you are adding a belt from a discipline." });
            }

            if (belt == null || belt.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Belt).Name, message = "Delete failed." });
            }

            return View(belt);
        }

        // Standard Post method for delete.
        [HttpPost]
        public async Task<IActionResult> Delete(Belt belt)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            try
            {
                belt = await _db.FindAsync<Belt>(belt.BeltID);

                if (belt is not null || belt.IsArchived == true)
                {
                    AssignModifier(belt);
                    belt.IsArchived = true;
                    _db.Belts.Update(belt);
                    _db.SaveChanges();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return RedirectToAction("Edit", "Discipline", new { Id = belt.DisciplineID });
        }

        // Standard Get method for edit. id = belt ID.
        public async Task<IActionResult> Edit(int? id)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (id == null)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Belt).Name, message = "Edit failed." });
            }

            Belt belt;
            Discipline discipline;

            try
            {
                belt = await _db.FindAsync<Belt>(id);
                discipline = FindDiscipline(belt.DisciplineID);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            if (discipline != null)
            {
                ViewBag.DisciplineName = discipline.Name;
            }
            else
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Belt).Name, message = "No discipline found. Please ensure you are adding a belt from a discipline." });
            }

            if (belt == null || belt.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Belt).Name, message = "Edit failed." });
            }

            return View(belt);
        }

        // Standard Get method for edit. id = discipline ID.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Belt belt)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (belt is not null && belt.IsArchived is false)
            {
                if (!ModelState.IsValid)
                {
                    return View(belt);
                }

                AssignModifier(belt);

                belt.DisciplineID = id;

                try
                {
                    _db.Belts.Update(belt);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }

            return RedirectToAction("Edit", "Discipline", new { Id = belt.DisciplineID });
        }

        /// <summary>
        /// Populates the list of disciplines and inserts them into a ViewBag variable.
        /// </summary>
        private void PopulateDisciplines()
        {
            var disciplines = _db.Disciplines.ToList();
            var discSelectList = new List<SelectListItem>();

            foreach (var disc in disciplines)
            {
                discSelectList.Add(
                    new SelectListItem { Text = disc.Name, Value = disc.DisciplineID.ToString() }
                );
            }

            ViewBag.Disciplines = discSelectList;
        }

        /// <summary>
        /// Locates the discipline associated with a belt.
        /// </summary>
        /// <param name="id">The discipline id to search on.</param>
        /// <returns>The discipline located for the belt.</returns>
        private Discipline FindDiscipline(int id)
        {
            var discipline = _db.Disciplines.Where(x => x.DisciplineID == id && x.IsArchived == false).FirstOrDefault();

            return discipline;
        }
    }
}
