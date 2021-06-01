using CWBFightClub.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CWBFightClub.Models
{
    public class Discipline : IBaseModel
    {
        /// <summary>
        /// ID of the Discipline.
        /// </summary>
        [Key]
        public int DisciplineID { get; set; }

        /// <summary>
        /// Gets or sets the collection of scheduled classes for the discipline.
        /// </summary>
        public List<ScheduledClass> ScheduledClasses { get; set; }

        /// <summary>
        /// Name of the Discipline.
        /// </summary>
        [Required]
        [StringLength(50)]
        [DisplayName("Discipline Name")]
        public string Name { get; set; }

        /// <summary>
        /// Description of the Discipline.
        /// </summary>
        [Required]
        [StringLength(1000)]
        [DisplayName("Description")]
        public string Description { get; set; }

        /// <summary>
        /// The default cost per month of the Discipline.
        /// </summary>
        [Required]
        [DisplayName("Default Cost Per Month")]
        [Column(TypeName = "decimal(8,2)")]
        public decimal DefaultCostPerMonth { get; set; }

        /// <summary>
        /// Gets or sets the list of enrollments.
        /// </summary>
        public List<Enrollment> Enrollments { get; set; }

        /// <summary>
        /// ID of the entity that created the Discipline.
        /// </summary>
        [Required]
        public int CreatedBy { get; set; }

        /// <summary>
        /// ID of the entity that modified the Discipline last.
        /// </summary>
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// Date the Discipline was created.
        /// </summary>
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Date the Discipline record was modified.
        /// </summary>
        [DisplayName("Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Indicates whether the Discipline record is archived.
        /// </summary>
        [DisplayName("Is Archived")]
        [DefaultValue(false)]
        public bool IsArchived { get; set; } = false;

        /// <summary>
        /// Gets or sets the list of belts associated with a given discipline.
        /// </summary>
        public virtual ICollection<Belt> Belts { get; set; }

        // Properties for the calendar.
        [DisplayName("Calendar Color")]
        public string CalendarColor { get; set; }
    }
}
