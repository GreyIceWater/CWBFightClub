using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWBFightClub.Utilities
{
    public static class DateTimeExtensions
    {
        // Another method to get total months.
        public static int TotalMonths(this DateTime start, DateTime end)
        {
            return (start.Year * 12 + start.Month) - (end.Year * 12 + end.Month);
        }

        /// <summary>
        /// Get the difference in months from one date to another.
        /// </summary>
        /// <param name="dt1">A date to compare.</param>
        /// <param name="dt2">A date to compare.</param>
        /// <returns>The number of months difference.</returns>
        public static int GetTotalMonthsFrom(this DateTime dt1, DateTime dt2)
        {
            DateTime earlyDate = (dt1 > dt2) ? dt2.Date : dt1.Date;
            DateTime lateDate = (dt1 > dt2) ? dt1.Date : dt2.Date;

            // Start with 1 month's difference and keep incrementing
            // until we overshoot the late date
            int monthsDiff = 1;
            while (earlyDate.AddMonths(monthsDiff) <= lateDate)
            {
                monthsDiff++;
            }

            return monthsDiff - 1;
        }
    }
}
