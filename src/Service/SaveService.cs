using Effektive_Praesentationen.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effektive_Praesentationen.Service
{
    public class SaveService
    {
        /// <summary>
        /// saves the files locally
        /// </summary>
        ///  <param name="fileToSave">files that are supposed to be saved</param>>
        public void SaveMedia(string[] filesToSave)
        {
            foreach (string fileToSave in filesToSave)
            {
                //wenn fileToSave nicht in state\\media liegt, dann kopiere es dorthin
                string mediaFolderPath = Environment.CurrentDirectory + "\\state\\media";
                string targetFilePath = Path.Combine(mediaFolderPath, Path.GetFileName(fileToSave));
                if (!File.Exists(targetFilePath))
                {
                    File.Copy(fileToSave, targetFilePath);
                }
            }

        }

        public void DeleteMedia(string[] filesToDelete)
        {
            foreach(string fileToDelete in filesToDelete)
            {
                string mediaFolderPath = Environment.CurrentDirectory + "\\state\\media";
                string targetFilePath = Path.Combine(mediaFolderPath, Path.GetFileName(fileToDelete));
                if (File.Exists(targetFilePath))
                {
                    File.Delete(targetFilePath);
                }
            }
        }
    }
}
