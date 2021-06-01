using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWBFightClub.Services
{
    /// <summary>
    /// Check to see if the user has access to the site.
    /// </summary>
    public class AccessChecker : IAccessChecker
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISession session;

        public AccessChecker(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.session = httpContextAccessor.HttpContext.Session;
        }

        /// <summary>
        /// Check to see if there is an actively signed in user.
        /// </summary>
        /// <returns></returns>
        public IActionResult CheckForAccess()
        {
            if (this.session.GetString("Username") == null)
            {
                return new RedirectToActionResult("Signin", "Account", new RouteValueDictionary
                {
                    {"controller", "Account" },
                    { "action", "Signin" }
                });
            }

            return null;
        }        
    }
}
