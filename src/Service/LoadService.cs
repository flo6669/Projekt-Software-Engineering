using Effektive_Praesentationen.Model;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Effektive_Praesentationen.Extension;

namespace Effektive_Praesentationen.Service
{
    public class LoadService
    {
        /// <summary>
        /// loads the media files from the zip file and adds them to the application
        /// </summary>
        public ObservableCollection<Chapter>? UnpackageMedia()
        {
            ObservableCollection<Chapter> chapterList = new ObservableCollection<Chapter>();
            string stateDirectoryPath = Environment.CurrentDirectory + "\\state";
            string zipFilePath = Environment.CurrentDirectory + "\\state\\media.zip";
            if(!Directory.Exists(stateDirectoryPath))   //in case the state folder doesn't exist
            {
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory,"state"));
            }
            if(!File.Exists(zipFilePath))    //in case the zip file doesn't exist
            {
                using(ZipArchive archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                {
                }
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
