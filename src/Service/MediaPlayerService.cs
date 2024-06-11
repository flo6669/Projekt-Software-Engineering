using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace Effektive_Praesentationen.Service
{
    public class MediaPlayerService
    {
        ///<summary>
        /// opens default media player for specific media type 
        /// shows  error, if default media player cannot be opened and returns to homepage
        /// interaction logic for media player (e.g. loop)
        /// </summary>

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public void OpenMediaPlayer(string filePath)
        {
            try
            {
                //open file as process
                Process openDMP = new Process();
                openDMP.StartInfo.FileName = "explorer.exe";
                openDMP.StartInfo.Arguments = filePath;
                openDMP.Start();

                //wait until loaded, so input can be given
                openDMP.WaitForInputIdle();

                ///TODO: input for loop in opened application
                ///for PowerPoint: f5 for fullscreen, use loop function or arrows with timer
                ///for Video: f11 for fullscreen, STRG+T for loop
                ///for PDF: f11 for fullscreen, loop functions with arrow and timer
            }
            catch (Win32Exception ex)
            {

                Console.WriteLine(ex.Message);
              

            }

        }

        


    }
}
