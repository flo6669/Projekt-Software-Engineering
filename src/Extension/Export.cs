using Effektive_Praesentationen.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace Effektive_Praesentationen.Extension
{
    /// <summary>
    /// Exports the current state to the selceted drive
    /// </summary>
    public class Export
    {
        const String topFolderName = "Effektive Praesentationen";
        static String newAppPath;
        /// <summary>
        /// Creates the folders for the export
        /// </summary>
        /// <param name="path">Absolute path to the folder</param>
        /// <exception cref="Exception">Throws an exception if the folders could not be created</exception>
        public static void ExportFolders(string path, ObservableCollection<Chapter> chapterList, bool onDesktop)
        {
            try
            {
                if (path == Path.GetPathRoot(Environment.CurrentDirectory))
                {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                 
                }
                Console.WriteLine("Exporting to: " + path);
                if (Directory.Exists(Path.Combine(path, topFolderName)))
                {
                    newAppPath = Path.Combine(path, topFolderName);
                    CopyMedia(newAppPath,chapterList);
                    CopyFonts(newAppPath);
                }
                else
                {
                    CreateFolder(path, topFolderName);
                    newAppPath = Path.Combine(path, topFolderName);
                    CreateFolder(newAppPath, "state");
                    CreateFolder(newAppPath + "\\state", "media");
                    CreateFolder(newAppPath, "fonts");
                    CopyFonts(newAppPath);
                    string exeDestination=CopyExe(newAppPath);
                    CopyMedia(newAppPath,chapterList);
                    CopyDlls(newAppPath);
                }
                if (onDesktop)
                {
                    MessageBox.Show("Export successfull", "Success", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show("Install successfull", "Success", MessageBoxButton.OK);
                }
                
            }
            catch (Exception)
            {
                Directory.Delete(newAppPath, true);
                if(onDesktop)
                {
                    MessageBox.Show("Export failed: Check the avaible space and your permissions", "Error", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show("Install failed: Check the avaible space and your permissions", "Error", MessageBoxButton.OK);
                }
                
            }
        }


        private static void CopyDlls(string newAppPath)
        {
            try
            {
                string[] dlls = { "CommunityToolkit.Mvvm.dll", "Effektive_Praesentationen.deps.json", "Effektive_Praesentationen.dll", "Effektive_Praesentationen.runtimeconfig.json", "MetadataExtractor.dll", "Microsoft.Extensions.DependencyInjection.Abstractions.dll", "Microsoft.Extensions.DependencyInjection.dll", "System.Management.dll", "XmpCore.dll","System.Drawing.Common.dll" };
                foreach (string dll in dlls)
                {
                    if(System.IO.File.Exists(Path.Combine(Environment.CurrentDirectory, dll)))
                        System.IO.File.Copy(Path.Combine(Environment.CurrentDirectory, dll), Path.Combine(newAppPath, dll), true);
                }
                if(Directory.Exists(Path.Combine(Environment.CurrentDirectory, "runtimes")))
                    CopyFolder(Path.Combine(Environment.CurrentDirectory, "runtimes"), Path.Combine(newAppPath, "runtimes"), true);
            }
            catch (Exception)
            {
                throw new Exception("Unable to copy dlls");
            }
        }

        /// <summary>
        /// Creates a folder, checks for the write permission of the selected folder
        /// </summary>
        /// <param name="path">Absolute path to the folder</param>
        /// <param name="folderName">Name of the folder</param>
        /// <returns>DirectoryInfo of the created folder</returns>
        /// <exception cref="Exception">Throws an exception if the folder could not be created</exception>
        private static DirectoryInfo CreateFolder(string path, string folderName)
        {
            try
            {
                return Directory.CreateDirectory(Path.Combine(path, folderName));
            }
            catch (Exception)
            {
                throw new Exception("Unable to create folder: " + folderName);
            }
        }

        /// <summary>
        /// Copies a folder somewhere else
        /// </summary>
        /// <param name="sourcePath">Absolute path to the source folder</param>
        /// <param name="destinationPath">Absolute path to the destination folder</param>
        /// <param name="recursive">If true, the folder will be copied recursively</param>
        /// <exception cref="Exception">Throws an exception if the folder could not be copied</exception>
        private static void CopyFolder(string sourcePath, string destinationPath, bool recursive)
        {
            try
            {
                var dir = new DirectoryInfo(sourcePath);
                if (!dir.Exists)
                {
                    throw new Exception($"Source folder does not exist: ${sourcePath}");
                }
                Directory.CreateDirectory(destinationPath);
                // get files and folders
                DirectoryInfo[] dirs = dir.GetDirectories();
                foreach (DirectoryInfo subdir in dirs)
                {
                    string subPath = Path.Combine(destinationPath, subdir.Name);
                    CopyFolder(subdir.FullName, subPath, true);
                }
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string filePath = Path.Combine(destinationPath, file.Name);
                    file.CopyTo(filePath, false);
                }
            }
            catch (Exception)
            {
                throw new Exception("Unable to copy folder");
            }
        }

        /// <summary>
        /// Copies the exe to the export folder
        /// </summary>
        /// <param name="path">Absolute path to the folder</param>
        /// <exception cref="Exception">Throws an exception if the exe could not be copied</exception>
        private static string CopyExe(string path)
        {
            try
            {
                string? processPath = Environment.ProcessPath;
                if (processPath == null)
                {
                    throw new Exception("Unable to get exe path");
                }
                string exePath = processPath;
                string destination = Path.Combine(path, "Effektive_Praesentation.exe");
                System.IO.File.Copy(exePath, destination, true);
                return destination;

            }
            catch (Exception)
            {
                throw new Exception("Unable to copy exe");
            }
        }

        /// <summary>
        /// Copies the fonts to the export folder
        /// </summary>
        /// <param name="path">Absolute path to the folder</param>
        /// <exception cref="Exception">Throws an exception if the fonts could not be copied</exception>
        private static void CopyFonts(string path)
        {
            try
            {
                string source= Path.Combine(Environment.CurrentDirectory, "fonts");
                string destination = Path.Combine(path, "fonts");
                string[] files = Directory.GetFiles(source);
                foreach (string file in files )
                {
                    string fileName= Path.GetFileName(file);
                    string destFile = Path.Combine(destination, fileName);
                    if (!System.IO.File.Exists(destFile))
                    {
                        System.IO.File.Copy(file, destFile);
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Unable to copy fonts");
            }
        }
        /// <summary>
        /// Copies the media zip to the export folder
        /// </summary>
        /// <param name="path">Absolute path to the folder</param>
        /// <exception cref="Exception">Throws an exception if the config could not be copied</exception>
        private static void CopyMedia(string path, ObservableCollection<Chapter> chapterList)
        {
            try
            {
                //delete old media in folder
                string mediaFolderPath = Path.Combine(path, "state", "media");
                foreach(string file in Directory.GetFiles(mediaFolderPath))
                {
                    System.IO.File.Delete(file);
                }
                //add new media
                foreach (Chapter chapter in chapterList)
                {
                    string savePath = Path.Combine(path, "state", "media",chapter.Title);
                    if (System.IO.File.Exists(chapter.Path))
                        System.IO.File.Copy(chapter.Path, savePath);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Unable to copy media"+ e);
            }
        }
    }
}
