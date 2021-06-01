using System;
using System.Collections.Generic;

namespace CWBFightClub.Models
{
    public class StudentBalanceReport
    {
        // Prompts

        // Output
        public List<StudentBalanceReportResult> Results { get; set; } = new List<StudentBalanceReportResult>();
    }

    public class StudentBalanceReportResult
    {
        public string TableStudent { get; set; }
        public decimal CurrentBalance { get; set; }
        public int StudentID { get; set; }
        public string StudentPhone { get; set; }
    }
}
