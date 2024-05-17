using Effektive_Praesentationen.Model;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effektive_Praesentationen.Service
{
    public class LoadService
    {
        /// <summary>
        /// loads the media files from the zip file and adds them to the application
        /// </summary>
        public List<Chapter>? UnpackageMedia()
        {
            List<Chapter> chapterList= new List<Chapter>();
            string zipFilePath = Environment.CurrentDirectory + "\\state\\media.zip";
            string savePath = Environment.CurrentDirectory + "\\state\\media";
            //if zip file does not exist, return
            if (!File.Exists(zipFilePath))
            {
                return chapterList;
            }
            try
            {
                ZipFile.ExtractToDirectory(zipFilePath, savePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Entpacken der Zip-Datei {ex.Message}");
            }
            //add every file from the media folder to the chapter list
            DirectoryInfo dir = new DirectoryInfo(savePath);
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Name != "config.json")
                {
                    chapterList.Add(new Chapter { Description = "Description", Title = file.Name, Loop = false });
                }
            }
            //delete zip file
            File.Delete(zipFilePath);
            return chapterList;
        }
    }
}
