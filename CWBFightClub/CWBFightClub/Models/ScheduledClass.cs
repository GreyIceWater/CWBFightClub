using CWBFightClub.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CWBFightClub.Models
{
    public class ScheduledClass : IBaseModel
    {
        /// <summary>
        /// Gets or sets the primary identifier for the class.
        /// </summary>
        [Key]
        public int ScheduledClassID { get; set; }

        /// <summary>
        /// Gets or sets the discipline ID of the class.
        /// </summary>
        public int DisciplineID { get; set; }

        /// <summary>
        /// Gets or sets the discipline of the class.
        /// Left nullable for occassions we may want to add a non-discipline event to the calendar.
        /// </summary>
        public Discipline Discipline { get; set; }

        /// <summary>
        /// Gets or sets the name of the class. Title.
        /// </summary>
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the start date and time of the class.
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Gets or sets the end date and time of the class.
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// Gets or sets the value indicating if there is recurrence.
        /// </summary>
        public bool HasRecurrence { get; set; }

        /// <summary>
        /// Gets or sets the recurrence frequency. Currently only supporting weekly.
        /// </summary>
        public string RecurrenceFrequency { get; set; }

        /// <summary>
        /// Gets or sets the recurrence time.
        /// </summary>
        public int RecurrenceTime { get; set; }

        /// <summary>
        /// Gets or sets the created date of the class.
        /// </summary>
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the modified date of the class.
        /// </summary>
        [DisplayName("Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the ID of the entity that created the class.
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the ID of the entity that modified the class.
        /// </summary>
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the value indicating if the class is archived.
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets the list of attendance records for the class.
        /// </summary>
        public List<AttendanceRecord> AttendanceRecords { get; set; }
    }
}
