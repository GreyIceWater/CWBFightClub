using CWBFightClub.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CWBFightClub.Models
{
    public class Enrollment : IBaseModel
    {
        /// <summary>
        /// Gets or sets the ID of the enrollment.
        /// </summary>
        [Key]
        public int EnrollmentID { get; set; }

        /// <summary>
        /// Gets or sets the ID of the discipline.
        /// </summary>
        public int DisciplineID { get; set; }

        /// <summary>
        /// Gets or sets the discipline.
        /// </summary>
        public Discipline Discipline { get; set; }

        /// <summary>
        /// Gets or sets the ID of the student.
        /// </summary>
        public int StudentID { get; set; }

        /// <summary>
        /// Gets or sets the student.
        /// </summary>
        public Student Student { get; set; }

        /// <summary>
        /// Gets or sets the list of achieved belts.
        /// </summary>
        public List<AchievedBelt> AchievedBelts { get; set; }

        /// <summary>
        /// Gets or sets the date the enrollment was started.
        /// </summary>
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the date the enrollment was ended.
        /// </summary>
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayName("Start Date")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the ID of the entity that created the record.
        /// </summary>
        [Required]
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the ID of the entity that modified the record.
        /// </summary>
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the date the record was created.
        /// </summary>
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date the record was modified.
        /// </summary>
        [DisplayName("Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the record is archived.
        /// </summary>
        [DisplayName("Is Archived")]
        [DefaultValue(false)]
        public bool IsArchived { get; set; } = false;
    }
}
