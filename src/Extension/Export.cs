using System.IO;

namespace Effektive_Praesentationen.Extension
{
    /// <summary>
    /// Exports the current state to the selceted drive
    /// </summary>
    public class Export
    {
        const String topFolderName = "Effektive Praesentationen";
        /// <summary>
        /// Creates the folders for the export
        /// </summary>
        /// <param name="path">Absolute path to the folder</param>
        /// <exception cref="Exception">Throws an exception if the folders could not be created</exception>
        public static void CreateExportFolders(string path)
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
                    var newAppPath = Path.Combine(path, topFolderName);
                    CopyZip(newAppPath);
                }
                else
                {
                    CreateFolder(path, topFolderName);
                    var newAppPath = Path.Combine(path, topFolderName);
                    CreateFolder(newAppPath, "state");
                    //CopyFonts(newAppPath);
                    CopyExe(newAppPath);
                    CopyZip(newAppPath);
                    CopyDlls(newAppPath);
                    DeleteLocalFiles();
                }
                Console.WriteLine("Export successful");
            }
            catch (Exception)
            {
                throw new Exception("Unable to create folders");
            }
        }


        private static void CopyDlls(string newAppPath)
        {
            try
            {
                string[] dlls = { "CommunityToolkit.Mvvm.dll", "Effektive_Praesentationen.deps.json", "Effektive_Praesentationen.dll", "Effektive_Praesentationen.pdb", "Effektive_Praesentationen.runtimeconfig.json", "MetadataExtractor.dll", "Microsoft.Extensions.DependencyInjection.Abstractions.dll", "Microsoft.Extensions.DependencyInjection.dll", "System.Management.dll", "XmpCore.dll" };
                foreach (string dll in dlls)
                {
                    File.Copy(Path.Combine(Environment.CurrentDirectory, dll), Path.Combine(newAppPath, dll), true);
                }
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
        private static void CopyExe(string path)
        {
            try
            {
                string? processPath = Environment.ProcessPath;
                if (processPath == null)
                {
                    throw new Exception("Unable to get exe path");
                }
                string exePath = processPath;
                File.Copy(exePath, Path.Combine(path, "Effektive_Praesentation.exe"), true);
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
                CopyFolder(Path.Combine(Environment.CurrentDirectory, "fonts"), Path.Combine(path, "fonts"), false);
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
        private static void CopyZip(string path)
        {
            try
            {
                File.Copy(Path.Combine(Environment.CurrentDirectory, "state\\media.zip"), Path.Combine(path, "state\\media.zip"), true);
            }
            catch (Exception)
            {
                throw new Exception("Unable to copy zip");
            }
        }

        private static void DeleteLocalFiles()
        {
            //Directory.Delete(Path.Combine(Environment.CurrentDirectory, "state\\media"),true);
            File.Delete(Path.Combine(Environment.CurrentDirectory, "state\\config.json"));
        }
    }
}
