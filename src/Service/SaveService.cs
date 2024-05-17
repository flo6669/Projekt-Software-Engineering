using Effektive_Praesentationen.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effektive_Praesentationen.Service
{
    public class SaveService
    {
        /// <summary>
        /// creates a zip file locally,
        /// also checks for duplicate files names and changes them
        /// </summary>
        ///  <param name="filesToZip">files that are supposed to be zipped</param>>
        public void PackageMedia(List<string> filesToZip)
        {
            string zipFilePath = Environment.CurrentDirectory + "\\state\\media.zip";
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
            foreach (string originFilePath in filesToZip)
            {
                string fileName = Path.GetFileName(originFilePath);
                string targetFilePath = Path.Combine(zipFilePath, fileName);
                string fileExtension = Path.GetExtension(originFilePath);
                while (filesAlreadyZipped.Contains(fileName))
                {
                    fileName = Path.GetFileNameWithoutExtension(originFilePath) + "(" + counter + ")" + fileExtension;
                    targetFilePath = Path.Combine(zipFilePath, fileName);
                    counter++;
                }
                zip.CreateEntryFromFile(originFilePath, fileName);
            }
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
    }
}
