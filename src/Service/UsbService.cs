using Effektive_Praesentationen.Model;
using Effektive_Praesentationen.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Effektive_Praesentationen.Service
{
    public class UsbService
    {
        private ManagementEventWatcher? _insertionWatcher;
        private ManagementEventWatcher? _removalWatcher;

        public ObservableCollection<UsbDrive> UsbDrives { get; set; } = new ObservableCollection<UsbDrive>();

        public event Action UpdateSelectedDrive;

        public UsbService()
        {
            InitializeWatcherInsert();
            InitializeWatcherRemoved();
            GetUsbInfo();
        }
        /// <summary>
        /// waits for usb stick insertion event
        /// </summary>
        private async Task InitializeWatcherInsert()
        {
            _insertionWatcher = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2");
            _insertionWatcher.EventArrived += Watcher_EventArrived;
            _insertionWatcher.Query = query;
            _insertionWatcher.Start();

            await Task.Run(() => _insertionWatcher.WaitForNextEvent());
        }

        /// <summary>
        /// waits for usb stick removal event
        /// </summary>
        public async Task InitializeWatcherRemoved()
        {
            _removalWatcher = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 3");
            _removalWatcher.EventArrived += Watcher_EventArrived;
            _removalWatcher.Query = query;
            _removalWatcher.Start();

            await Task.Run(() => _removalWatcher.WaitForNextEvent());
        }

        /// <summary>
        /// function that is executed when a usb stick is inserted or removed
        /// </summary>
        private void Watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            GetUsbInfo();
            UpdateSelectedDrive?.Invoke();
        }

        /// <summary>
        /// gets information of all removable drives
        /// </summary>
        public void GetUsbInfo()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            Application.Current.Dispatcher.Invoke(() => UsbDrives.Clear());
            foreach (DriveInfo drive in drives)
            {
                if (drive.DriveType == DriveType.Removable)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UsbDrives.Add(new UsbDrive
                        {
                            Name = drive.Name,
                            VolumeLabel = drive.VolumeLabel,
                            DriveFormat = drive.DriveFormat,
                            TotalSize = drive.TotalSize / 1000000,
                            TotalFreeSpace = drive.AvailableFreeSpace / 1000000
                        });
                    });
                }
            }
            if(UsbDrives.Count==0)
                Application.Current.Dispatcher.Invoke(() => UsbDrives.Add(new UsbDrive { Name = "Please insert drive" }));  
        }
    }
}
