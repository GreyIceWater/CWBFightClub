using CWBFightClub.Data;
using CWBFightClub.Models;
using CWBFightClub.Services;
using CWBFightClub.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWBFightClub.Controllers
{
    public class AccountController : BaseController
    {
        // Constants used for Accounts.
        private const string USERNAME_ERROR = "Username not found.";
        private const string PASSWORD_ERROR = "Password is incorrect.";
        private const string DUPLICATE_USERNAME = "Username already exists: ";
        private const string PASSWORD_REENTRY_CHECK = "Passwords do not match. Please re-enter passwords.";
        private const string ACCOUNT_LOCKED = "This account is currently locked. Please wait 15 minutes and try again. " +
            "If the problem continues please contact a CWB administrator to resolve the issue.";
        private const string RELATED_ID_ERROR = "User already has a registered account.";
        private const string APPROVAL_PENDING = "Account is inactive, awaiting approval.";
        private const string USERNAME_COOKIE = "CWBFightClubUsername";
        private const string PASSWORD_COOKIE = "CWBFightClubPassword";

        public AccountController(CWBContext db, IAccessChecker ac) : base(ac, db)
        {
        }

        // GET: Accounts
        public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber = 1)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            IEnumerable<Account> accounts = null;

            await Task.Run(() =>
            {
                accounts = _db.Accounts.Where(x => x.IsArchived == false).Include(x => x.Student).AsNoTracking();
            });

            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentSearch"] = searchString;

            ViewData["FirstNameSortParm"] = sortOrder == "FirstName" ? "firstname_desc" : "FirstName";
            ViewData["LastNameSortParm"] = sortOrder == "LastName" ? "lastname_desc" : "LastName";
            ViewData["UserNameSortParm"] = sortOrder == "UserName" ? "username_desc" : "UserName";

            switch (sortOrder)
            {
                case "FirstName":
                    accounts = accounts.OrderBy(a => a.Student.FirstName);
                    break;
                case "firstname_desc":
                    accounts = accounts.OrderByDescending(a => a.Student.FirstName);
                    break;
                case "LastName":
                    accounts = accounts.OrderBy(a => a.Student.LastName);
                    break;
                case "lastname_desc":
                    accounts = accounts.OrderByDescending(a => a.Student.LastName);
                    break;
                case "UserName":
                    accounts = accounts.OrderBy(a => a.Username);
                    break;
                case "username_desc":
                    accounts = accounts.OrderByDescending(a => a.Username);
                    break;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                accounts = accounts.Where(a => a.Student.FirstName.ToLower().Contains(searchString.ToLower()) ||
                            a.Student.LastName.ToLower().Contains(searchString.ToLower()));
            }

            return View(PaginatedList<Account>.Create(accounts.AsQueryable(), pageNumber.Value, SystemConstants.ItemsPerPage));
        }

        // GET: Accounts/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.InstructorsExist = await PopulateStudentSelect();
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(Account account)
        {
            if (ModelState.IsValid)
            {
                List<Account> accountsList = await _db.Accounts.Where(x => x.IsArchived == false).ToListAsync();

                // Check for duplicate username.
                if (accountsList.Where(x => x.Username == account.Username).Any())
                {
                    account.Password = null;
                    account.Password2 = null;
                    ModelState.AddModelError("Username", DUPLICATE_USERNAME + account.Username);
                    ViewBag.InstructorsExist = await PopulateStudentSelect();
                    return this.View(account);
                }

                // Check for re-entered password.
                if (account.Password != account.Password2)
                {
                    account.Password = null;
                    account.Password2 = null;
                    ModelState.AddModelError("Password", PASSWORD_REENTRY_CHECK);
                    ViewBag.InstructorsExist = await PopulateStudentSelect();
                    return this.View(account);
                }



                IPasswordHasher pwHasher = new PasswordHasher();

                account.Password = pwHasher.Hash(account.Password);

                AssignCreator(account);

                _db.Accounts.Add(account);
                await _db.SaveChangesAsync();

                return RedirectToAction("Index", "Account");
            }
            else
            {
                //ModelState.AddModelError("", "Please correct highlighted errors in the form.");
                ViewBag.InstructorsExist = await PopulateStudentSelect();
                return View(account);
            }
        }

        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (id == null)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Account).Name, message = "Edit failed." });
            }

            var account = await _db.Accounts.FindAsync(id);

            if (account == null || account.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Account).Name, message = "Edit failed." });
            }

            await _db.Entry(account).Reference(x => x.Student).LoadAsync();
            await PopulateStudentSelect(account.StudentID);
            account.Password = null;
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Account account)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (account == null)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Account).Name, message = "Edit failed." });
            }

            var errors = ModelState["Password"].Errors;
            bool bypassPasswordValidation = false;

            if (account.Password == null && errors[0].ErrorMessage == "Entry is required.")
            {
                ModelState["Password"].Errors.Clear();

                bypassPasswordValidation = true;
                ModelState["Password"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Account acctFromDb = await _db.Accounts.Where(x => x.AccountID == account.AccountID).FirstOrDefaultAsync();

                    IEnumerable<Account> accountsNotUs = _db.Accounts.Where(x => x.AccountID != account.AccountID && x.IsArchived == false);

                    bool duplicateUserNameFound = accountsNotUs.Any(x => x.Username == account.Username);

                    // Check for duplicate username.
                    if (duplicateUserNameFound)
                    {
                        account.Password = null;
                        account.Password2 = null;
                        ModelState.AddModelError("Username", DUPLICATE_USERNAME + account.Username);
                        await PopulateStudentSelect(account.StudentID);
                        return this.View(account);
                    }

                    // Check for re-entered password.
                    if (account.Password != account.Password2)
                    {
                        account.Password = null;
                        account.Password2 = null;
                        ModelState.AddModelError("Password", PASSWORD_REENTRY_CHECK);
                    }

                    if (!ModelState.IsValid)
                    {
                        ModelState.AddModelError("", "Please correct highlighted errors in the form.");
                        await PopulateStudentSelect(account.StudentID);
                        return View(account);
                    }

                    IPasswordHasher pwHasher = new PasswordHasher();
                    if (bypassPasswordValidation)
                    {
                        acctFromDb.StudentID = account.StudentID;
                        acctFromDb.Username = account.Username;

                        AssignModifier(acctFromDb);
                        _db.Accounts.Update(acctFromDb);
                        await _db.SaveChangesAsync();

                    }
                    else
                    {
                        if (await TryUpdateModelAsync(acctFromDb, "",
                        x => x.StudentID,
                        x => x.Username,
                        x => x.Password))
                        {
                            acctFromDb.Password = pwHasher.Hash(account.Password);
                            AssignModifier(acctFromDb);
                            _db.Accounts.Update(acctFromDb);
                            await _db.SaveChangesAsync();
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountID) || account.IsArchived == true)
                    {
                        return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Account).Name, message = "Edit failed." });
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            await PopulateStudentSelect(account.StudentID);
            return View(account);
        }

        // GET: Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (id == null)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Account).Name, message = "Delete failed." });
            }

            var account = await _db.Accounts
                .FirstOrDefaultAsync(m => m.AccountID == id);

            if (account == null || account.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Account).Name, message = "Delete failed." });
            }

            await _db.Entry(account).Reference(x => x.Student).LoadAsync();

            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Account account)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (account == null)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Account).Name, message = "Delete failed." });
            }

            var accountFound = await _db.Accounts.FindAsync(account.AccountID);

            if (accountFound == null || accountFound.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Account).Name, message = "Delete failed." });
            }

            accountFound.IsArchived = true;
            AssignModifier(accountFound);

            try
            {
                _db.Accounts.Update(accountFound);
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(accountFound.AccountID))
                {
                    return RedirectToAction("ObjectNotFound", "Base", new { type = typeof(Account).Name, message = "Deletion failed." });
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _db.Accounts.Any(e => e.AccountID == id);
        }

        public IActionResult SignIn()
        {
            return View();
        }

        /// <summary>
        /// Signs users in to CWBFightClub
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Signin(Account account)
        {
            IEnumerable<Account> accounts = await _db.Accounts.ToListAsync();
            IPasswordHasher pwHasher = new PasswordHasher();

            Account accountFound = accounts.Where(x => x.Username == account.Username && x.IsArchived == false).FirstOrDefault();

            if (accountFound == null || accountFound.IsArchived == true)
            {
                ModelState.AddModelError("Username", USERNAME_ERROR);
                account.Password = null;
                account.Password2 = null;
                return View("Signin", account);
            }

            bool passwordValid = pwHasher.Check(accountFound.Password, account.Password);

            if (!passwordValid)
            {
                ModelState.AddModelError("Password", PASSWORD_ERROR);
                account.Password = null;
                account.Password2 = null;
                return View("Signin", account);
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please correct highlighted errors in the form.");
                return View("Signin", account);
            }

            if (account.IsRemembered)
            {
                SetPersistentCookie(USERNAME_COOKIE, account.Username);
                SetPersistentCookie(PASSWORD_COOKIE, accountFound.Password);
            }
            else
            {
                DeletePersistentCookie(USERNAME_COOKIE);
                DeletePersistentCookie(PASSWORD_COOKIE);
            }

            this.UpdateSession(accountFound);
            return this.RedirectToAction("Index", "Student");
        }

        /// <summary>
        /// Signs the user out. Destroys session storage for the user.
        /// </summary>
        /// <returns>The home index action is returned.</returns>
        [HttpGet]
        public IActionResult Signout()
        {
            HttpContext.Session.Clear();
            DeletePersistentCookie(USERNAME_COOKIE);
            DeletePersistentCookie(PASSWORD_COOKIE);

            return RedirectToAction("Signin", "Account");
        }



        /// <summary>
        /// Gets a list of students without accounts.
        /// </summary>
        /// <param name="editStudentID">If editting an account, pass in current associated StudentID so it can be included in student list.</param>
        /// <returns>The list of students that are instructors without accounts.</returns>
        private async Task<List<Student>> GetStudentListWithoutAccounts(int editStudentID)
        {
            List<int> linkedAccountsId = await _db.Accounts.Where(x => x.IsArchived == false).Select(x => x.StudentID).ToListAsync();
            List<Student> students = await _db.Students.Where(x => !linkedAccountsId.Contains(x.StudentID) && x.IsArchived == false && x.IsInstructor == true).ToListAsync();

            if (editStudentID != 0)
            {
                students.Add(await _db.Students.Where(x => x.StudentID == editStudentID).FirstOrDefaultAsync());
            }

            return students;
        }

        /// <summary>
        /// Populates the select list for students who are instructors that don't have an account.
        /// </summary>        
        /// <param name="editStudentID">Optional: If editting an account, pass in current associated StudentID so it can be included in select list.</param>
        /// <returns>True if entries exist.</returns>
        private async Task<bool> PopulateStudentSelect(int editStudentID = 0)
        {
            bool successful = true;

            try
            {
                List<Student> students = await GetStudentListWithoutAccounts(editStudentID);
                var studentSelectList = new List<SelectListItem>();

                if (students.Count() == 0)
                {
                    return false;
                }
                else
                {
                    foreach (Student student in students)
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

        /// <summary>
        /// Updates session. Use the following on views if needed.
        /// @using Microsoft.AspNetCore.Http 
        /// @inject IHttpContextAccessor HttpContextAccessor
        /// to restrict
        /// @if(Convert.ToInt32(HttpContextAccessor.HttpContext.Session.GetString("AccessLevel")) >= (int) AccessLevel.Admin)
        /// </summary>
        /// <param name="account"></param>
        private void UpdateSession(Account account)
        {
            HttpContext.Session.SetString("Username", account.Username);
            HttpContext.Session.SetString("AccountId", account.AccountID.ToString());
        }

        private void SetPersistentCookie(string key, string value)
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddHours(4);
            Response.Cookies.Append(key, value, option);
        }

        private void DeletePersistentCookie(string key)
        {
            Response.Cookies.Delete(key);
        }
    }
}