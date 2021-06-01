using CWBFightClub.Data;
using CWBFightClub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CWBFightClub.Controllers
{
    public class FilePathsController : Controller
    {
        private readonly CWBContext _context;

        public FilePathsController(CWBContext context)
        {
            _context = context;
        }

        // GET: FilePaths
        public async Task<IActionResult> Index()
        {
            var cWBContext = _context.FilePaths.Include(f => f.Student);
            return View(await cWBContext.ToListAsync());
        }

        // GET: FilePaths/Create
        public IActionResult Create()
        {
            ViewData["StudentID"] = new SelectList(_context.Students, "StudentID", "FirstName");
            return View();
        }

        // POST: FilePaths/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FilePathId,FileName,FileType,DateCreated,Comment,StudentID")] FilePath filePath)
        {
            if (ModelState.IsValid)
            {
                _context.Add(filePath);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StudentID"] = new SelectList(_context.Students, "StudentID", "FirstName", filePath.StudentID);
            return View(filePath);
        }

        // GET: FilePaths/Edit/5
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

            ViewBag.oldpath = filePath.FileName;
            ViewBag.extension = Path.GetExtension(filePath.FileName);

            ViewData["StudentID"] = new SelectList(_context.Students, "StudentID", "FirstName", filePath.StudentID);
            return View(filePath);
        }

        // POST: FilePaths/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FilePathId,FileName,FileType,DateCreated,Comment,StudentID")] FilePath filePath, string oldPath, string ext)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    filePath.FileName = filePath.FileName + ext;
                    _context.Update(filePath);

                    string pathconcat = "wwwroot/Images/" + oldPath;

                    if (filePath.FileName != oldPath)
                    {

                        FileSystem.RenameFile(pathconcat, filePath.FileName);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilePathExists(filePath.FilePathId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "Attachments", new { studentid = filePath.StudentID });

            }
            ViewData["StudentID"] = new SelectList(_context.Students, "StudentID", "FirstName", filePath.StudentID);
            return RedirectToAction("Index", "Attachments", new { studentid = filePath.StudentID });
        }

        // GET: FilePaths/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filePath = await _context.FilePaths
                .Include(f => f.Student)
                .FirstOrDefaultAsync(m => m.FilePathId == id);
            if (filePath == null)
            {
                return NotFound();
            }

            return View(filePath);
        }

        // POST: FilePaths/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var filePath = await _context.FilePaths.FindAsync(id);
            _context.FilePaths.Remove(filePath);
            await _context.SaveChangesAsync();

            // Delete the file
            string pathconcat = "wwwroot/Images/" + filePath.FileName;

            if ((System.IO.File.Exists(pathconcat)))
            {
                System.IO.File.Delete(pathconcat);
            }
            else
            {
                return Redirect("~/Attachments/Index?studentid=" + filePath.StudentID);

            }


            return Redirect("~/Attachments/Index?studentid=" + filePath.StudentID);
        }

        private bool FilePathExists(int id)
        {
            return _context.FilePaths.Any(e => e.FilePathId == id);
        }
    }
}
