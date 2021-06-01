using System;
using System.Collections.Generic;

namespace CWBFightClub.Models
{
    public class StudentPaymentReport
    {
        // Prompts
        public int DaysUntilPaymentIsDue { get; set; }

        // Output
        public List<StudentPaymentReportResult> Results { get; set; } = new List<StudentPaymentReportResult>();
        public string GraphDatas { get; set; }
        public string GraphLabels { get; set; }
    }

    public class StudentPaymentReportResult
    {
        public string TableStudent { get; set; }
        public DateTime DueDate { get; set; }
        public decimal AmountDue { get; set; }
        public decimal CurrentBalance { get; set; }
        public int StudentID { get; set; }
        public string StudentPhone { get; set; }
    }
}
