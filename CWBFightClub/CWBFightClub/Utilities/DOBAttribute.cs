using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CWBFightClub.Utilities
{
    /// <summary>
    /// The class used for date of birth annotations.
    /// </summary>
    public class DOBAttribute : RangeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the DOBAttribute class.
        /// </summary>
        public DOBAttribute()
        : base(typeof(DateTime), DateTime.Now.AddYears(SystemConstants.MaxAge * -1).ToShortDateString(), DateTime.Now.AddYears(SystemConstants.MinAge * -1).ToShortDateString()) 
        {
        }
    }
}
