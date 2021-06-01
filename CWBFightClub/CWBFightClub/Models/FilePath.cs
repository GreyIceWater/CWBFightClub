using System;
using System.ComponentModel.DataAnnotations;

namespace CWBFightClub.Models
{
    public class FilePath
    {
        /// <summary>
        /// Gets or sets the primary identifier for the file path.
        /// </summary>
        [Key]
        public int FilePathId { get; set; }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        [StringLength(255), Required]
        public string FileName { get; set; }    

        /// <summary>
        /// Gets or sets the file type.
        /// </summary>
        public FileType FileType { get; set; }

        /// <summary>
        /// Gets or sets the date of creation.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the comment associated with the file path.
        /// </summary>
        [StringLength(300)]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the studentID related to the file path.
        /// </summary>
        public int StudentID { get; set; }

        /// <summary>
        /// Gets or sets the student related to the file path.
        /// </summary>
        public virtual Student Student { get; set; }
    }
}
