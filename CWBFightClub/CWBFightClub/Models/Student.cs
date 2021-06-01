using CWBFightClub.Models.Interfaces;
using CWBFightClub.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CWBFightClub.Models
{
    /// <summary>
    /// The class used to represent a student or student instructor.
    /// </summary>
    public class Student : IBaseModel
    {
        private string phone;

        /// <summary>
        /// Gets or sets the primary identifier for the student.
        /// </summary>
        [Key]
        public int StudentID { get; set; }

        /// <summary>
        /// The attendance records for the student.
        /// </summary>
        public List<AttendanceRecord> AttendanceRecords { get; set; }

        /// <summary>
        /// Gets or sets a first name.
        /// </summary>
        [Required]
        [StringLength(50)]
        [DisplayName("First Name")]
        [RegularExpression(@"^[a-zA-Z'-]+$", ErrorMessage = "Only letters, dashes and apostrophes allowed.")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets a middle name.
        /// </summary>
        [StringLength(50)]
        [DisplayName("Middle Name")]
        [RegularExpression(@"^[a-zA-Z'-]+$", ErrorMessage = "Only letters, dashes and apostrophes allowed.")]
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets a last name.
        /// </summary>
        [Required]
        [StringLength(50)]
        [DisplayName("Last Name")]
        [RegularExpression(@"^[a-zA-Z'-]+$", ErrorMessage = "Only letters, dashes and apostrophes allowed.")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the date of birth of the student.
        /// </summary>
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DOB(ErrorMessage = "Value for {0} must be between {1:d} and {2:d}")]
        public DateTime DOB { get; set; }

        /// <summary>
        /// Gets or sets the street address part of the student's address.
        /// </summary>
        [StringLength(100)]
        [DisplayName("Street Address")]
        [RegularExpression(@"^[0-9a-zA-Z#() ,.'-]+$", ErrorMessage = "Only letters, numbers, spaces, commas, periods, apostrophes, dashes, # and parathesis are allowed.")]
        public string StreetAddress { get; set; }

        /// <summary>
        /// Gets or sets the city part of the student's address.
        /// </summary>
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z]+(?:[\s-][a-zA-Z]+)*$", ErrorMessage = "Invalid city name.")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the two letter state abbreviation part of the student's address.
        /// </summary>
        [StringLength(2)]
        [RegularExpression(@"^[A-Z]{2}", ErrorMessage = "Invalid state entry.")]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the ZIP code part of the student's address.
        /// </summary>
        [StringLength(10)]
        [Column(TypeName = "varchar(10)")]
        [RegularExpression(@"(^\d{5}$)|(^\d{9}$)|(^\d{5}-\d{4}$)", ErrorMessage = "Please enter a valid 5 or 9 digit ZIP Code")]
        public string ZIP { get; set; }

        /// <summary>
        /// Gets or sets the student's phone number.
        /// </summary>
        [StringLength(15)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number. Please use format ### ###-####")]
        public string Phone
        {
            get
            {
                return this.phone;
            }
            set
            {
                if (value != null)
                {
                    this.phone = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the student's email address.
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the student signed a waiver.
        /// </summary>
        [DisplayName("Waiver Signed")]
        [DefaultValue(false)]
        public bool WaiverSigned { get; set; }

        /// <summary>
        /// Gets or sets the name of the file of the waiver stored in the server's storage.
        /// </summary>
        public ICollection<FilePath> FilePaths { get; set; }

        /// <summary>
        /// Gets or sets the amount of an alternative payment agreement.
        /// </summary>
        [Column(TypeName = "decimal(8,2)")]
        [DisplayName("Payment Amount")]
        public decimal? PaymentAgreementAmount { get; set; }

        /// <summary>
        /// Gets or sets the interval between payments.
        /// </summary>
        public PaymentPeriod PaymentAgreenmentPeriod { get; set; }

        /// <summary>
        /// Gets or sets the note about an alternative payment agreement.
        /// </summary>
        [StringLength(255)]
        [DisplayName("Payment Note")]
        public string PaymentAgreementNote { get; set; }

        /// <summary>
        /// Gets or sets the balance due amount.
        /// </summary>
        [Column(TypeName = "decimal(8,2)")]
        [DisplayName("Balance Due")]
        public decimal? BalanceDue { get; set; }

        /// <summary>
        /// Gets or sets the balance due date.
        /// </summary>
        [DisplayName("Balance Due Date")]
        public DateTime? BalanceDueDate { get; set; }

        /// <summary>
        /// Gets or sets the balance due date.
        /// </summary>
        [DisplayName("Balance Modified By System Date")]
        public DateTime? BalanceModifiedBySystemDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the student is an instructor.
        /// </summary>
        [DisplayName("Is Instructor")]
        [DefaultValue(false)]
        public bool IsInstructor { get; set; }

        /// <summary>
        /// Gets or sets the guardians of a student. Virtual for lazy loading.
        /// </summary>
        public virtual IEnumerable<StudentGuardian> StudentGuardians { get; set; }

        /// <summary>
        /// Gets or sets the list of enrollments.
        /// </summary>
        public List<Enrollment> Enrollments { get; set; }

        /// <summary>
        /// Gets or sets the list of payments.
        /// </summary>
        public List<Payment> Payments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the student is archived.
        /// </summary>
        [DisplayName("Is Archived")]
        [DefaultValue(false)]
        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets the created date of the student record.
        /// </summary>
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the modified date of the student record.
        /// </summary>
        [DisplayName("Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the ID of the entity that created the student record.
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the ID of the entity that modified the student record.
        /// </summary>
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the checked in status of the student.
        /// </summary>
        [NotMapped]
        public bool IsCheckedIn { get; set; }

        /// <summary>
        /// Gets or sets the active attendance record of the student.
        /// </summary>
        [NotMapped]
        public AttendanceRecord ActiveAttendanceRecord { get; set; }
    }
}
