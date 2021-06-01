using CWBFightClub.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CWBFightClub.Models
{
    /// <summary>
    /// The class which represents a Guardian.
    /// </summary>
    public class Guardian : IBaseModel
    {
        private string phone;

        /// <summary>
        /// Gets or sets the ID of the Guardian.
        /// </summary>
        [Key]
        public int GuardianID { get; set; }

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
        /// Gets or sets the street address of the Guardian.
        /// </summary>
        [Required]
        [StringLength(100)]
        [DisplayName("Street Address")]
        [RegularExpression(@"^[0-9a-zA-Z#() ,.'-]+$", ErrorMessage = "Only letters, numbers, spaces, commas, periods, apostrophes, dashes, # and parathesis are allowed.")]
        public string StreetAddress { get; set; }

        /// <summary>
        /// Gets or sets the city part of the guardian's address.
        /// </summary>
        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z]+(?:[\s-][a-zA-Z]+)*$", ErrorMessage = "Invalid city name.")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the two letter state abbreviation part of the guardian's address.
        /// </summary>
        [Required]
        [StringLength(2)]
        [RegularExpression(@"^[A-Z]{2}", ErrorMessage = "Invalid state entry.")]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the ZIP code part of the guardian's address.
        /// </summary>
        [Required]
        [StringLength(10)]
        [Column(TypeName = "varchar(10)")]
        [RegularExpression(@"(^\d{5}$)|(^\d{9}$)|(^\d{5}-\d{4}$)", ErrorMessage = "Please enter a valid 5 or 9 digit ZIP Code")]
        public string ZIP { get; set; }

        /// <summary>
        /// Gets or sets the student's phone number.
        /// </summary>
        [Required]
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
                this.phone = value;
            }
        }

        /// <summary>
        /// Gets or sets the email address of the Guardian.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the ID of the entity that created the Guardian.
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the ID of the entity that modified the Guardian last.
        /// </summary>
        public int? ModifiedBy { get; set; } = null;

        /// <summary>
        /// Gets or sets the date the Guardian was created.
        /// </summary>
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date the Guardian record was modified.
        /// </summary>
        [DisplayName("Modified Date")]
        public DateTime? ModifiedDate { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether the Guardian record is archived.
        /// </summary>        
        [DisplayName("Is Archived")]
        [DefaultValue(false)]
        public bool IsArchived { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating if the guardian is the primary contact for the student.
        /// </summary>
        [NotMapped]
        [DisplayName("Primary")]
        [DefaultValue(false)]
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Gets or sets the relationship between student and guardian.
        /// </summary>
        [NotMapped]
        [StringLength(50)]
        [DisplayName("Relationship To Student")]
        [RegularExpression(@"^[a-zA-Z'-]+$", ErrorMessage = "Only letters, dashes and apostrophes allowed.")]
        public string Relationship { get; set; }

        /// <summary>
        /// Gets or sets the list of associated StudentGuardian records.
        /// </summary>
        public ICollection<StudentGuardian> StudentGuardians { get; set; }
    }
}
