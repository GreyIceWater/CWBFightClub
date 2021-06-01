using CWBFightClub.Models.Interfaces;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CWBFightClub.Models
{
    public class AppSetting : IBaseModel
    {
        /// <summary>
        /// Gets or sets the id of the record.
        /// </summary>
        [Key]
        public int AppSettingID { get; set; }

        /// <summary>
        /// Gets or sets the bundle cost for a month.
        /// </summary>
        [Required]
        [DisplayName("Monthly Gym Package")]
        [Column(TypeName = "decimal(8,2)")]
        [Range(0, 999999)]
        public decimal BundleCostPerMonth { get; set; }

        /// <summary>
        /// Gets or sets the bundle cost for 3 months.
        /// </summary>
        [Required]
        [DisplayName("3-Month Gym Package")]
        [Column(TypeName = "decimal(8,2)")]
        [Range(0, 999999)]
        public decimal BundleCostPerThreeMonths { get; set; }

        /// <summary>
        /// Gets or sets the bundle cost for a year.
        /// </summary>
        [Required]
        [DisplayName("Yearly Gym Package")]
        [Column(TypeName = "decimal(8,2)")]
        [Range(0, 999999)]
        public decimal BundleCostPerYear { get; set; }

        [Required]
        [DisplayName("Percent of Class Required To Count As Attended")]
        [Range(0, 100)]
        public int PercentOfClassRequiredToVerify { get; set; }

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
