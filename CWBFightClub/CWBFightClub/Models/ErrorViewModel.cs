using System;

namespace CWBFightClub.Models
{
    /// <summary>
    /// The class which contains field structure for the error view model.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Gets or sets the RequestedId from the error.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Returns the ShowRequestId if one exists.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);
    }
}
