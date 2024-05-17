using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Effektive_Praesentationen.Extension;
using Effektive_Praesentationen.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Effektive_Praesentationen.Model;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Management;
using System.Windows;
using System.IO.Compression;

namespace Effektive_Praesentationen.ViewModel
{
    public partial class FileSelectionViewModel : Core.ViewModel, IFilesDropped, IOpenFileDialog
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FileValid))]
        [NotifyCanExecuteChangedFor(nameof(NavigateToInactiveLoopCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportCommand))]
        private Chapters _chapters;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FileValid))]
        [NotifyCanExecuteChangedFor(nameof(NavigateToInactiveLoopCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportCommand))]
        private string? _change; //only there because changing Chapters.DefaultChapter does not trigger the NotifyPropertyChangedFor

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DriveChosen))]
        [NotifyPropertyChangedFor(nameof(CanExport))]
        [NotifyCanExecuteChangedFor(nameof(ExportCommand))]
        private UsbDrive? _selectedDrive;

        [ObservableProperty]
        private INavigationService _navigation;

        [ObservableProperty]
        public UsbService _usbService;

        [ObservableProperty]
        public SaveService _saveService;

        [ObservableProperty]
        public LoadService _loadService;


        public FileSelectionViewModel(INavigationService navService)
        {
            Chapters = new Chapters();
            Navigation = navService;
            UsbService = new UsbService();
            SaveService = new SaveService();
            LoadService = new LoadService();
            viewName = "FileSelection";
            Import();
        }

        /// <summary>
        /// command to navigate to the inactive loop view
        /// </summary>
        [RelayCommand(CanExecute = nameof(FileValid))]
        public void NavigateToInactiveLoop()
        {
            Navigation.PathNavigateTo<InactiveLoopViewModel>(Chapters.DefaultChapter);

            if (Navigation.CurrentViewModel is InactiveLoopViewModel inactiveLoopViewModel)
            {
                inactiveLoopViewModel.FilePath = Chapters.DefaultChapter;
            }
        }

        /// <summary>
        /// true, when a file is selected, false otherwise
        /// </summary>
        public bool FileValid
        {
            get
            {
                if (String.IsNullOrEmpty(Chapters.DefaultChapter))
                {
                    return false;
                }
                string fileExtension = Path.GetExtension(Chapters.DefaultChapter);
                if (!(fileExtension == ".mp4" || fileExtension == ".mkv" || fileExtension == ".mov"))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// true, when a drive is selected, false otherwise
        /// </summary>
        public bool DriveChosen
        {
          get
            {
                if (SelectedDrive == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// true, when file and drive are selected, false otherwise
        /// </summary>
        public bool CanExport
        {
            get
            {
                if (DriveChosen && FileValid)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public void OnFilesDropped(string[] files)
        {
            Chapters.DefaultChapter = files[0];
            Change = files[0];
        }

        public void OnFileSelected(string[] files)
        {
            Chapters.DefaultChapter = files[0];
            Change = files[0];
        }      

        /// <summary>
        /// executes all methods to export the data to the usb stick
        /// </summary>
        [RelayCommand(CanExecute =nameof(CanExport))]
        public async Task Export()
        {   
            //files to zip= Chaperts.DefaultChapter and config.json
            List<string> filesToZip= new List<string>();
            filesToZip.Add(Chapters.DefaultChapter);
            foreach (Chapter chapter in Chapters.ChapterList)
            {
                filesToZip.Add(".\\state\\media\\"+chapter.Title);
            }
            await Task.Run(() => SaveService.SaveSettings(filesToZip));
            filesToZip.Add(Environment.CurrentDirectory + "\\state\\config.json");
            await Task.Run(() => SaveService.PackageMedia(filesToZip));
            string rootPath = Path.GetPathRoot(Environment.CurrentDirectory);
            Extension.Export.CreateExportFolders(SelectedDrive.Name);
        }

        /// <summary>
        /// executes all methods needed to load the data into the application
        /// </summary>
        public async Task Import()
        { 
            Chapters.ChapterList=await Task.Run(() => LoadService.UnpackageMedia());
        }

    }
}
