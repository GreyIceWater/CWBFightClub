using CWBFightClub.Data;
using CWBFightClub.Models;
using CWBFightClub.Models.Interfaces;
using CWBFightClub.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CWBFightClub.Controllers
{
    public class BaseController : Controller
    {
        protected CWBContext _db;

        public BaseController(IAccessChecker ac, CWBContext db)
        {
            this.AccessChecker = ac;
            this._db = db;
        }

        protected IAccessChecker AccessChecker { get; set; }

        /// <summary>
        /// Assigns the creator information to a student record.
        /// </summary>
        /// <param name="student">The student to add creation information to.</param>
        protected void AssignCreator<T>(T dataObject) where T : IBaseModel
        {
            dataObject.CreatedBy = GetCurrentAccountId();
            dataObject.CreatedDate = DateTime.Now;
        }

        protected void AssignModifier<T>(T dataObject) where T : IBaseModel
        {
            dataObject.ModifiedBy = GetCurrentAccountId();
            dataObject.ModifiedDate = DateTime.Now;
        }

        public int GetCurrentAccountId()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("AccountId"));

            return userId;
        }

        public string GetCurrentAccountUsername()
        {
            string username = HttpContext.Session.GetString("Username");

            return username;
        }        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Handle404Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ObjectNotFound(string type, string message = null)
        {            
            return View(new ObjectNotFoundViewModel { ObjectType = type, CustomMessage = message });
        }

        /// <summary>
        /// Archive associated guardian if no active studentguardian records exist.
        /// </summary>
        /// <param name="guardianid">The guardian that will be archived if studentguardian records no longer active.</param>
        protected async Task CheckForStudentGuardianRecords(int guardianid)
        {
            try
            {
                IEnumerable<StudentGuardian> studentGuardians = null;
                await Task.Run(() =>
                {
                    studentGuardians = _db.StudentGuardians.Where(x => x.IsArchived == false && x.GuardianID == guardianid);
                });

                // If there are no active StudentGuardian records, archive the guardian.
                if (!studentGuardians.Any())
                {
                    Guardian guardian = await _db.FindAsync<Guardian>(guardianid);
                    AssignModifier(guardian);
                    guardian.IsArchived = true;

                    _db.Guardians.Update(guardian);
                    await _db.SaveChangesAsync();
                }
            }
            catch(DbUpdateConcurrencyException)
            {
                throw;
            }
            catch(Exception)
            {
                throw;
            }           
        }

        /// <summary>
        /// Archive list of guardians if no active studentguardian records exist.
        /// </summary>
        /// <param name="guardianIds">Collection of guardianIds to archive.</param>
        protected async Task CheckForStudentGuardianRecords(ICollection<int> guardianIds)
        {
            try
            {
                foreach(int g in guardianIds)
                {
                    IEnumerable<StudentGuardian> studentGuardians = null;
                    await Task.Run(() =>
                    {
                        studentGuardians = _db.StudentGuardians.Where(x => x.IsArchived == false && x.GuardianID == g);
                    });

                    // If there are no active StudentGuardian records, archive the guardian.
                    if (!studentGuardians.Any())
                    {
                        Guardian guardian = await _db.FindAsync<Guardian>(g);
                        AssignModifier(guardian);
                        guardian.IsArchived = true;

                        _db.Guardians.Update(guardian);
                    }
                }

                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
