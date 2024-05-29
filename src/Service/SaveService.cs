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
        private readonly object _zipLock = new object();
        /// <summary>
        /// creates a zip file locally,
        /// also checks for duplicate files names and changes them
        /// </summary>
        ///  <param name="fileToZip">file that is supposed to be zipped</param>>
        public string PackageMedia(string fileToZip)
        {
            string zipFilePath = Environment.CurrentDirectory + "\\state\\media.zip";
            string fileName = Path.GetFileName(fileToZip);
            lock (_zipLock) //make sure only one thread can access the zip file at a time
            {
                using ZipArchive zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Update);
                //delete old json config file in zip file
                ZipArchiveEntry? oldConfig = zip.GetEntry("config.json");
                if (oldConfig != null)
                {
                    oldConfig.Delete();
                }

                //check for duplicate file names
                string[] filesAlreadyZipped = zip.Entries.Select(entry => entry.FullName).ToArray();
                int counter = 1;
                string targetFilePath = Path.Combine(zipFilePath, fileName);
                string fileExtension = Path.GetExtension(fileToZip);
                while (filesAlreadyZipped.Contains(fileName))
                {
                    fileName = Path.GetFileNameWithoutExtension(fileToZip) + "(" + counter + ")" + fileExtension;
                    targetFilePath = Path.Combine(zipFilePath, fileName);
                    counter++;
                }
                zip.CreateEntryFromFile(fileToZip, fileName);
                
            }
            return fileName;
        }

        /// <summary>
        /// saves settings locally in a json file
        /// </summary>
        public void SaveSettings(List<string> filePaths)
        {
            string savePath = Environment.CurrentDirectory + "\\state\\config.json";
            //delete old json config file
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            //convert Chapter List in json string
            foreach(string filePath in filePaths)
            {
                string jsonString = System.Text.Json.JsonSerializer.Serialize(Path.GetFileName(filePath));
                //write json string to file
                File.WriteAllText(savePath, jsonString);
            }
        }

        public void DeleteChapter(string chapterName)
        {
            string zipFilePath = Environment.CurrentDirectory + "\\state\\media.zip";
            lock(_zipLock) //make sure only one thread can access the zip file at a time
            {
                using ZipArchive zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Update);
                ZipArchiveEntry? entryToDelete = zip.GetEntry(chapterName);
                if (entryToDelete != null)
                {
                    entryToDelete.Delete();
                }
            }
        }
    }
}
