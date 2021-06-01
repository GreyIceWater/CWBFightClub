using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWBFightClub.Services
{
    /// <summary>
    /// Interface used to check for site access.
    /// </summary>
    public interface IAccessChecker
    {
        IActionResult CheckForAccess();
    }
}
