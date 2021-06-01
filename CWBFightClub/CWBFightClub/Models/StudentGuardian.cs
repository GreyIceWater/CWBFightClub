using CWBFightClub.Models.Interfaces;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CWBFightClub.Models
{
    /// <summary>
    /// The class which represents a StudentGuardian.
    /// </summary>
    public class StudentGuardian : IBaseModel
    {
        /// <summary>
        /// Gets or sets the ID of the StudentGuardian record.
        /// </summary>
        [Key]
        public int StudentGuardianID { get; set; }

        /// <summary>
        /// Gets or sets the associated Guardian ID.
        /// </summary>
        public int GuardianID { get; set; }

        /// <summary>
        /// Gets or sets the associated Student ID.
        /// </summary>
        public int StudentID { get; set; }

        /// <summary>
        /// Gets or sets the guardian object associated.
        /// </summary>
        public Guardian Guardian { get; set; }

        /// <summary>
        /// Gets or sets the student object associated.
        /// </summary>
        public Student Student { get; set; }

        /// <summary>
        /// Gets or sets the ID of the entity that created the Guardian.
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the ID of the entity that modified the StudentGuardian last.
        /// </summary>
        public int? ModifiedBy { get; set; } = null;

        /// <summary>
        /// Gets or sets the date the StudentGuardian was created.
        /// </summary>
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date the StudentGuardian record was modified.
        /// </summary>
        [DisplayName("Modified Date")]
        public DateTime? ModifiedDate { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether the StudentGuardian record is archived.
        /// </summary>
        [DisplayName("Is Archived")]
        [DefaultValue(false)]
        public bool IsArchived { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating if the guardian is the primary contact for the student.
        /// </summary>
        [DisplayName("Primary")]
        [DefaultValue(false)]
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Gets or sets the relationship between student and guardian.
        /// </summary>
        [StringLength(50)]
        [DisplayName("Relationship To Student")]
        [RegularExpression(@"^[a-zA-Z'-]+$", ErrorMessage = "Only letters, dashes and apostrophes allowed.")]
        public string Relationship { get; set; }
    }
}
