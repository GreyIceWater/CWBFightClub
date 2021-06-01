using CWBFightClub.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CWBFightClub.Models
{
    public class AttendanceRecord : IBaseModel, IValidatableObject
    {
        /// <summary>
        /// Gets or sets the primary identifier for the class.
        /// </summary>
        [Key]
        public int AttendanceRecordID { get; set; }

        /// <summary>
        /// Gets or sets the scheduled class ID.
        /// </summary>
        public int ScheduledClassID { get; set; }

        /// <summary>
        /// Gets or sets the scheduled class.
        /// </summary>
        public ScheduledClass ScheduledClass { get; set; }

        /// <summary>
        /// Gets or sets the student ID.
        /// </summary>
        [Required]
        public int StudentID { get; set; }

        /// <summary>
        /// Gets or sets the student of the record.
        /// </summary>
        public Student Student { get; set; }

        /// <summary>
        /// Gets or sets the start date and time.
        /// </summary>
        [Display(Name = "Start Date"), DataType(DataType.DateTime), Required]
        public DateTime Start { get; set; }

        /// <summary>
        /// Gets or sets the end date and time.
        /// </summary>
        [Display(Name = "End Date"), DataType(DataType.DateTime), Required]
        public DateTime End { get; set; }

        /// <summary>
        /// Gets or sets the value indicating if  the attendance record is verified.
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the modified date.
        /// </summary>
        [DisplayName("Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the ID of the entity that created.
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the ID of the entity that modified.
        /// </summary>
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the value indicating if the record is archived.
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Custom server side validation for End and Start properties.
        /// </summary>
        /// <param name="validationContext">Context in which validation is performed.</param>
        /// <returns>Returns validation results.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            if (End < Start)
            {
                errors.Add(new ValidationResult($"{nameof(End)} date/time needs to be after Start date/time", new List<string> { nameof(End) }));
            }

            if (Start > DateTime.Now)
            {
                errors.Add(new ValidationResult($"{nameof(Start)} date/time cannot be in the future", new List<string> { nameof(Start) }));
            }
            return errors;
        }
    }
}
