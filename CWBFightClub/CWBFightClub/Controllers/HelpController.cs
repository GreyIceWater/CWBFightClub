using Microsoft.AspNetCore.Mvc;

namespace CWBFightClub.Controllers
{
    public class HelpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
