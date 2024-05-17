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
        private string _fileToAdd;

        [ObservableProperty]
        public string? _feedbackText;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DriveChosen))]
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
                if (String.IsNullOrEmpty(FileToAdd))
                {
                    return false;
                }
                string fileExtension = Path.GetExtension(FileToAdd);
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

        public async Task OnFilesDropped(string[] files)
        {
            FileToAdd = files[0];
            if(FileValid)
            {
                Chapters.ChapterList.Add(new Chapter { Description = "Description", Title = Path.GetFileName(files[0]), Loop = false });
                await Task.Run(() => SaveService.PackageMedia(files.ToList()));
                FeedbackText = "Datei erfolgreich hinzugefügt";
            }
            else
            {
                FeedbackText = "Fehler: Dateiendung nicht unterstützt";
            }
        }

        public async Task OnFileSelected(string[] files)
        {
            FileToAdd = files[0];
            FeedbackText = "";
            if (FileValid)
            {
                Chapters.ChapterList.Add(new Chapter { Description = "Description", Title = Path.GetFileName(files[0]), Loop = false });
                await Task.Run(() => SaveService.PackageMedia(files.ToList()));
                FeedbackText = "Datei erfolgreich hinzugefügt";
            }
            else
            {
                FeedbackText = "Fehler: Dateiendung nicht unterstützt";
            }
        }      

        /// <summary>
        /// executes all methods to export the data to the usb stick
        /// </summary>
        [RelayCommand(CanExecute =nameof(DriveChosen))]
        public async Task Export()
        {   
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
