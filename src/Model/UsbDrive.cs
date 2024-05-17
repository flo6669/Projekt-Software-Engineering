using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effektive_Praesentationen.Model
{
    public class UsbDrive
    {
        public string? Name { get; set; }
        public string? VolumeLabel { get; set; }
        public string? DriveFormat { get; set; }
        public long TotalSize { get; set; }
        public long TotalFreeSpace { get; set; }
        public override string ToString()
        {
            if (Path.GetPathRoot(Environment.CurrentDirectory) == Name)
                return "Desktop";
            else
                return $"{Name}{VolumeLabel}  Freier Platz: {TotalFreeSpace} MB";
        }
    }
}