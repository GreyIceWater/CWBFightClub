using CWBFightClub.Models.Interfaces;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CWBFightClub.Models
{
    public class Belt : IBaseModel
    {
        /// <summary>
        /// ID of the Belt.
        /// </summary>
        [Key]
        public int BeltID { get; set; }

        /// <summary>
        /// Associated Discipline ID.
        /// </summary>
        public int DisciplineID { get; set; }

        /// <summary>
        /// Name of the Belt.
        /// </summary>
        [Required]
        [StringLength(50)]
        [DisplayName("Belt Name")]
        public string Name { get; set; }

        /// <summary>
        /// Description of the Belt.
        /// </summary>
        [Required]
        [StringLength(100)]
        [DisplayName("Belt Description")]
        public string BeltDescription { get; set; }

        /// <summary>
        /// The rank of the belt.
        /// </summary>
        [Required]
        [DisplayName("Rank")]
        public int Rank { get; set; }

        /// <summary>
        /// Description of the Rank.
        /// </summary>
        [Required]
        [StringLength(100)]
        [DisplayName("Rank Description")]
        public string RankDescription { get; set; }

        /// <summary>
        /// ID of the entity that created the Belt.
        /// </summary>
        [Required]
        public int CreatedBy { get; set; }

        /// <summary>
        /// ID of the entity that modified the Belt last.
        /// </summary>
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// Date the Belt was created.
        /// </summary>
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Date the Belt record was modified.
        /// </summary>
        [DisplayName("Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Indicates whether the Belt record is archived.
        /// </summary>
        [DisplayName("Is Archived")]
        [DefaultValue(false)]
        public bool IsArchived { get; set; } = false;
    }
}
