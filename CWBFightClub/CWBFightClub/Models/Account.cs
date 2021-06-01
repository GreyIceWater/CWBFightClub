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
    public class Account : IBaseModel
    {
        /// <summary>
        /// Gets or sets the ID of the account.
        /// </summary>
        [Key]
        public int AccountID { get; set; }

        /// <summary>
        /// Gets or sets the associated StudentID.
        /// </summary>
        [ForeignKey("Student")]
        public int StudentID { get; set; }

        /// <summary>
        /// Gets or sets the student of the account.
        /// </summary>
        public Student Student { get; set; }

        /// <summary>
        /// Gets or sets the username of the account.
        /// </summary>
        [Required(ErrorMessage = "Entry is required.")]
        [RegularExpression(@"^[0-9a-zA-Z_]+$", ErrorMessage = "Only numbers, letters and underscores allowed.")]
        [MaxLength(50, ErrorMessage = "Must be 50 characters or less.")]
        [MinLength(3, ErrorMessage = "Must be 3 or more characters.")]
        [StringLength(50)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password of the account.
        /// </summary>
        [Required(ErrorMessage = "Entry is required.")]
        [PasswordPropertyText]
        [MaxLength(50, ErrorMessage = "Must be 50 characters or less.")]
        [MinLength(5, ErrorMessage = "Must be 5 or more characters.")]
        [StringLength(255)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the password string re-entered on creation.
        /// </summary>
        [NotMapped]
        //[Required(ErrorMessage = "Entry is required.")]
        [PasswordPropertyText]
        [MaxLength(50, ErrorMessage = "50 characters or less.")]
        [MinLength(5, ErrorMessage = "Must be 5 or more characters.")]
        [DisplayName("Re-Enter Password")]
        public string Password2 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not an account login is remembered.
        /// </summary>
        [NotMapped]
        public bool IsRemembered { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the account is archived.
        /// </summary>
        [DisplayName("Is Archived")]
        [DefaultValue(false)]
        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets the created date of the account.
        /// </summary>
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the modified date of the account.
        /// </summary>
        [DisplayName("Modified Date")]
        public DateTime? ModifiedDate { get; set; } = null;

        /// <summary>
        /// Gets or sets the ID of the entity that created the account.
        /// </summary>
        [Required]
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the ID of the entity that modified the account.
        /// </summary>
        public int? ModifiedBy { get; set; } = null;
    }
}
