using System;
using System.Collections.Generic;

namespace CWBFightClub.Models
{
    public class StudentAttendanceReport
    {
        // Prompts
        public int DaysSinceLastAttendanceRecord { get; set; } // Days Since Last Active
        public bool IncludeInstructors { get; set; }
        public bool IncludeOnlyActiveEnrollments { get; set; }

        // Output
        public List<StudentAttendanceReportResult> Results { get; set; } = new List<StudentAttendanceReportResult>();
        public string GraphDatas { get; set; }
        public string GraphLabels { get; set; }
    }

    public class StudentAttendanceReportResult
    {
        public string TableStudent { get; set; }
        public int TableDaysSinceLastAttendance { get; set; }
        public DateTime DateOfLastAttendance { get; set; }
        public string LastDisciplineAttended { get; set; }
        public int StudentID { get; set; }
        public string StudentPhone { get; set; }
    }
}
