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
using System.Drawing.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows;
using System.IO.Enumeration;

namespace Effektive_Praesentationen.Service
{
    public class LoadService
    {
        /// <summary>
        /// loads the media files from the zip file and adds them to the application
        /// </summary>
        public ObservableCollection<Chapter>? LoadMedia()
        {
            ObservableCollection<Chapter> chapterList = new ObservableCollection<Chapter>();
            if(!Directory.Exists(Environment.CurrentDirectory + "\\state"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\state");
            }
            if (!Directory.Exists(Environment.CurrentDirectory + "\\state\\media"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\state\\media");
            }
            //get files from media folder
            string mediaFolderPath = Environment.CurrentDirectory + "\\state\\media";
            if (Directory.Exists(mediaFolderPath))
            {
                string[] files = Directory.GetFiles(mediaFolderPath);
                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    chapterList.Add(new Chapter {
                        Path = Environment.CurrentDirectory + "\\state\\media\\"+fileName, Title = fileName
                    });
                }
            }
            return chapterList;
        }

        public void LoadFonts()
        {
            if (!Directory.Exists(Environment.CurrentDirectory + "\\fonts"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\fonts");
            }

            string[] fontNames = { "ZEISS Frutiger Next W1G" };
            bool fontsMissing = false;
            foreach (string fontName in fontNames)
            {
                if (FontIsInstalled(fontName))
                {
                    string fontPath = GetFontFilePath(fontName);
                    if (!File.Exists(Environment.CurrentDirectory + "\\fonts\\" + fontName + ".ttf"))
                    {
                        //copy font to folder
                        File.Copy(fontPath, Environment.CurrentDirectory + "\\fonts\\" + fontName +".ttf");
                    }
                }
                else if(File.Exists(Environment.CurrentDirectory + "\\fonts\\" + fontName + ".ttf"))
                {
                    //install font if not already in user font directory
                    if(!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Windows\Fonts", fontName))){
                        InstallFont(Environment.CurrentDirectory + "\\fonts\\" + fontName + ".ttf", fontName);
                    }
                }
                //else give warning with message box
                else
                {
                    fontsMissing = true;
                }
            }
            if(fontsMissing)
            {
                MessageBox.Show("Some fonts are missing. They won't be displayed in the media.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        /// <summary>
        /// Checks if a font is installed
        /// </summary>
        /// <param name="fontName">name of the font</param>
        /// <returns>true, when font is installed</returns>
        static bool FontIsInstalled(string fontName)
        {
            using (InstalledFontCollection installedFonts = new InstalledFontCollection())
            {
                {
                    return installedFonts.Families.Any(fontFamily => fontFamily.Name.Equals(fontName, StringComparison.OrdinalIgnoreCase));
                }
            }
        }

        static void InstallFont(string fontPath, string fontName)
        {
            string userFontsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Windows\Fonts");
            Directory.CreateDirectory(userFontsDirectory); // create directory, if not existing
            string destinationPath = Path.Combine(userFontsDirectory, Path.GetFileName(fontPath));

            try
            {
                // copy font file in user folder
                if (!File.Exists(destinationPath))
                {
                    File.Copy(fontPath, destinationPath);
                }

                // add font to registry for current user
                string fontValue = Path.GetFileName(destinationPath);
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Fonts", true);
                key.SetValue(fontName, fontValue, RegistryValueKind.String);
                key.Close();

                // refresh system cache
                AddFontResource(destinationPath);
                SendMessage(HWND_BROADCAST, WM_FONTCHANGE, IntPtr.Zero, IntPtr.Zero);

                Console.WriteLine($"The font '{fontName}' was sucessfully installed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error installing font: {ex.Message}");
            }
        }

        // methods for refreshing system cache
        [DllImport("gdi32.dll")]
        private static extern int AddFontResource(string lpFileName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const int HWND_BROADCAST = 0xffff;
        private const int WM_FONTCHANGE = 0x001D;

        static string GetFontFilePath(string fontName)
        {
            // path to registry key for installed fonts
            string fontsRegistryPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts";

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(fontsRegistryPath))
            {
                if (key != null)
                {
                    foreach (string fontRegKey in key.GetValueNames())
                    {
                        // check if registry name == searched font name
                        string fontFileName = key.GetValue(fontRegKey) as string;
                        if (fontRegKey.Contains(fontName, StringComparison.OrdinalIgnoreCase) && fontFileName != null)
                        {
                            string fontFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), fontFileName);
                            return fontFilePath;
                        }
                    }
                }
            }
            return null;
        }
    }
}
