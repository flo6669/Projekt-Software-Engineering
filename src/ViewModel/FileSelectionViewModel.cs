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
        [NotifyPropertyChangedFor(nameof(ChapterChosen))]
        [NotifyCanExecuteChangedFor(nameof(DeleteChapterCommand))]
        private Chapter? _selectedChapter;

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
            UsbService.UpdateSelectedDrive += UpdateSelectedDrive;
            Application.Current.Dispatcher.Invoke(() => SelectedDrive = UsbService.UsbDrives.Last());
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
                List<string> validExtensions = new List<string> { ".mp4", ".mkv", ".mov", ".pdf",".pptx" };
                if (!validExtensions.Contains(fileExtension))
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
                if (SelectedDrive == null || SelectedDrive.VolumeLabel == null) //VolumeLabel is null for placeholder
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
        /// true, when a chapter is selected, false otherwise
        /// </summary>
        public bool ChapterChosen
        {
            FileToAdd = files[0];
            if(FileValid)
            {
                if (SelectedChapter == null)
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
                string fileName=await Task.Run(() => SaveService.PackageMedia(files[0]));
                Chapters.ChapterList.Add(new Chapter { Description = "Description", Title = Path.GetFileName(fileName), Loop = false });
                FeedbackText = "File successfully added";
            }
            else
            {
                FeedbackText = "Error: File extension not supported";
            }
        }

        public async Task OnFileSelected(string[] files)
        {
            FileToAdd = files[0];
            FeedbackText = "";
            if (FileValid)
            {
                string fileName=await Task.Run(() => SaveService.PackageMedia(files[0]));
                Chapters.ChapterList.Add(new Chapter { Description = "Description", Title = Path.GetFileName(fileName), Loop = false });
                FeedbackText = "File successfully added";
            }
            else
            {
                FeedbackText = "Error: File extension not supported";
            }
        }
        
        public void UpdateSelectedDrive()
        {
            Application.Current.Dispatcher.Invoke(() => SelectedDrive = UsbService.UsbDrives.Last());
        }

        /// <summary>
        /// executes all methods to export the data to the usb stick
        /// </summary>
        [RelayCommand(CanExecute =nameof(DriveChosen))]
        public async Task Export()
        {
            MessageBoxResult result = MessageBox.Show("Do you want to export the data to the selected drive?", "Export", MessageBoxButton.YesNo);
            if (result==MessageBoxResult.Yes)
            {
                await Task.Run(() => Extension.Export.CreateExportFolders(SelectedDrive.Name));
                UsbService.GetUsbInfo();
                UpdateSelectedDrive();
            }
        }

        /// <summary>
        /// executes all methods needed to load the data into the application
        /// </summary>
        public async Task Import()
        { 
            Chapters.ChapterList=await Task.Run(() => LoadService.UnpackageMedia());
        }

        [RelayCommand(CanExecute =nameof(ChapterChosen))]
        public async Task DeleteChapter()
        {
            await Task.Run(() => SaveService.DeleteChapter(SelectedChapter.Title));
            Chapters.ChapterList.Remove(SelectedChapter);
            FeedbackText = "File successfully removed";
        }

    }
}
