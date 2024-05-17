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
            //if zip file does not exist, return
            if (!File.Exists(zipFilePath))
            {
                return chapterList;
            }
            using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName != "config.json")
                    {
                        chapterList.Add(new Chapter { Description = "Description", Title = entry.FullName, Loop = false });
                    }
                }
            }
            return chapterList;
        }
    }
}
