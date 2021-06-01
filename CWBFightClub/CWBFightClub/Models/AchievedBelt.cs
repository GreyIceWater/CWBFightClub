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
    
    public class AchievedBelt : IBaseModel
    {
        /// <summary>
        /// Gets or sets the ID of the archieved belt.
        /// </summary>
        [Key]
        public int AchievedBeltID { get; set; }

        /// <summary>
        /// Gets or sets the ID of the enrollment.
        /// </summary>
        [Required]
        public int EnrollmentID { get; set; }

        /// <summary>
        /// Gets or sets the name of the achieved belt.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the achieved belt.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the rank of the achieved belt.
        /// </summary>
        [Required]
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets the date the belt was achieved.
        /// </summary>
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime DateAchieved { get; set; }

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
