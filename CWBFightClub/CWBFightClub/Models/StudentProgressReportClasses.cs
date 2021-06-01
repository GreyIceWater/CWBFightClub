using System;
using System.Collections.Generic;

namespace CWBFightClub.Models
{
    public class StudentProgressReportClasses
    {
        // Prompts
        public int DisciplineID { get; set; }
        public string Discipline { get; set; }
        public int StudentClassesPastCurrentRank { get; set; }

        // Output
        public List<StudentProgressReportClassesResult> Results { get; set; } = new List<StudentProgressReportClassesResult>();
        public string GraphDatas { get; set; }
        public string GraphLabels { get; set; }
    }

    public class StudentProgressReportClassesResult
    {
        public string Student { get; set; }
        public int ClassCount { get; set; }
        public DateTime DateOfLastRank { get; set; }
        public int StudentID { get; set; }
    }
}
