using CWBFightClub.Models;
using CWBFightClub.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CWBFightClub.Services
{
    public class DirectoryUtility
    {
        /// <summary>
        /// Creates the directory location to save backups if it does not exist.
        /// </summary>
        public static void CreateDirectory()
        {
            string root = SystemConstants.RootPath;
            string subdirectory = SystemConstants.SubdirectoryPath;

            try
            {
                // Create the root directory if it does not exist.
                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }

                // Create sub-directory if it does not exist.
                // Split this out in case the subdirectory is altered or missing, a new one will be created.
                if (!Directory.Exists(subdirectory))
                {
                    Directory.CreateDirectory(subdirectory);
                }
            }
            catch (IOException ex)
            {
                throw new IOException("Database backup directory could not be created. Error: ", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred creating the file directory. Message: ", ex);
            }
        }        
    }
}
