using System;
using System.Collections.Generic;

namespace CWBFightClub.Models
{
    public class StudentProgressReport
    {
        // Prompts
        public int DisciplineID { get; set; }
        public string Discipline { get; set; }
        public int StudentHoursPastCurrentRank { get; set; }

        // Output
        public List<string> TableDatas { get; set; }
        public List<string> TableLabels { get; set; }
        public List<DateTime> DateOfLastRanks { get; set; }
        public List<int> StudentIDs { get; set; }
        public string GraphDatas { get; set; }
        public string GraphLabels { get; set; }
    }
}
