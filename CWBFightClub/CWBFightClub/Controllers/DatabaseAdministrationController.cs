using CWBFightClub.Data;
using CWBFightClub.Models;
using CWBFightClub.Services;
using CWBFightClub.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CWBFightClub.Controllers
{
    public class DatabaseAdministrationController : BaseController
    {
        public DatabaseAdministrationController(IAccessChecker ac, CWBContext db) : base(ac, db)
        {
        }        

        /// <summary>
        /// The index view for Database Administration.
        /// </summary>
        /// <returns>The Index view.</returns>
        public IActionResult Index()
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            List<DatabaseAdministration> files = GetFiles();

            return View(files);
        }
        
        /// <summary>
        /// Creates a backup of the CWBFightClub database.
        /// </summary>
        /// <returns>All backup files in the directory.</returns>
        public IActionResult Backup()
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            DirectoryUtility.CreateDirectory();

            _db.ExecuteDatabaseBackupUSP();

            return RedirectToAction("Index", "DatabaseAdministration");
        }

        /// <summary>
        /// Permanently deletes a given database backup file.
        /// </summary>
        /// <param name="filename">Name of file to delete.</param>
        /// <returns>A Redirect to the index action.</returns>
        public IActionResult Delete(string filename)
        {
            List<DatabaseAdministration> files = new();
            DatabaseAdministration fileToDelete = new();
            files = GetFiles();

            foreach (DatabaseAdministration file in files)
            {
                if (file.FileName == filename)
                {
                    try
                    {
                        System.IO.File.Delete($"{SystemConstants.SubdirectoryPath}/{file.FileName}");
                    }
                    catch(DirectoryNotFoundException DirectoryNotFound)
                    {
                        throw new DriveNotFoundException("Directory was not found to delete from. Message: ", DirectoryNotFound);
                    }
                    catch(Exception ex)
                    {
                        throw new Exception("The file could not be deleted. Message: ", ex);
                    }
                }
                    
            }

            return RedirectToAction("Index", "DatabaseAdministration");
        }

        /// <summary>
        /// Gets all files in the backups folder.
        /// </summary>
        /// <returns>A list of files that exist in the backup folder.</returns>
        private List<DatabaseAdministration> GetFiles()
        {
            string path = SystemConstants.SubdirectoryPath;

            //List<string> files = new List<string>();
            List<DatabaseAdministration> files = new();
            try
            {
                if (Directory.Exists(path))
                {
                    // Get the files in the given directory.
                    string[] fileEntries = Directory.GetFiles(path);
                    foreach (string file in fileEntries)
                    {
                        DatabaseAdministration da = new();
                        da.FileName = Path.GetFileName(file);
                        // Fully qualified call to File because ControllerBase already has a File method which conflicted.
                        da.FileDate = System.IO.File.GetLastWriteTime(file);
                        da.FileDate.ToString("yyyyMMddmmss");
                        da.FileLocation = path;

                        files.Add(da);
                    }
                }

                return files;
            }
            catch (IOException ex)
            {
                throw new IOException("An error occurred fetching all files. Message: ", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred. Message: ", ex);
            }
        }        
    }
}
