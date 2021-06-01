using CWBFightClub.Models.Interfaces;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CWBFightClub.Models
{
    public class Payment : IBaseModel
    {
        /// <summary>
        /// Gets or sets the id of the record.
        /// </summary>
        [Key]
        public int PaymentID { get; set; }

        /// <summary>
        /// Gets or sets the student id of the record.
        /// </summary>
        public int StudentID { get; set; }

        /// <summary>
        /// Gets or sets the date/time the payment was received.
        /// </summary>
        [Required]
        [DisplayName("Payment Date")]
        public DateTime ReceivedDate { get; set; }

        /// <summary>
        /// Gets or sets the payment amount.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(8,2)")]
        [Range(0.01, 9999.99)]
        public decimal Amount { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        [StringLength(255)]
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the entity that created the record.
        /// </summary>
        [Required]
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the entity that modified the record.
        /// </summary>
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the date/time the record was created.
        /// </summary>
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date/time the record was modified.
        /// </summary>
        [DisplayName("Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Indicates whether the record is archived.
        /// </summary>
        [DisplayName("Is Archived")]
        [DefaultValue(false)]
        public bool IsArchived { get; set; } = false;
    }
}
