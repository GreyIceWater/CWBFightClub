using CWBFightClub.Data;
using CWBFightClub.Models;
using CWBFightClub.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CWBFightClub.Controllers
{
    /// <summary>
    /// The class used for the HomeController.
    /// </summary>
    public class HomeController : BaseController
    {
        /// <summary>
        /// The logger to be used by the HomeController class.
        /// </summary>
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Initializes a new instance of the HomeController class.
        /// </summary>
        /// <param name="logger">The logger passed to assign to the HomeController logger.</param>
        public HomeController(ILogger<HomeController> logger, IAccessChecker ac, CWBContext db) : base(ac, db)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Loads the Index view.
        /// </summary>
        /// <returns>The index view is returned.</returns>
        public IActionResult Index()
        {
            return RedirectToAction("Signin", "Account");
        }

        /// <summary>
        /// Loads the Privacy view.
        /// </summary>
        /// <returns>The privacy view is returned.</returns>
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
