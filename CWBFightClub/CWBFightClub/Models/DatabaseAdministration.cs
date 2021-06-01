using CWBFightClub.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CWBFightClub.Models
{
    public class DatabaseAdministration
    {
        /// <summary>
        /// Gets or sets the File name from directory.
        /// </summary>
        [DisplayName("File Name")]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the File location of the directory.
        /// </summary>
        [DisplayName("File Location")]
        public string FileLocation { get; set; }

        /// <summary>
        /// Gets or sets the File date from directory.
        /// </summary>
        [DisplayName("File Date")]
        public DateTime FileDate { get; set; }
    }
}