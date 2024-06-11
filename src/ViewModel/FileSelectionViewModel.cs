﻿using CommunityToolkit.Mvvm.ComponentModel;
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
using System.Windows.Input;
using System.IO.Compression;
using System.Diagnostics;


namespace Effektive_Praesentationen.ViewModel
{
    public partial class FileSelectionViewModel : Core.ViewModel, IFilesDropped, IOpenFileDialog
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ChapterChosen))]
        [NotifyCanExecuteChangedFor(nameof(DeleteChapterCommand))]
        [NotifyCanExecuteChangedFor(nameof(OpenDefaultMediaPlayerCommand))]
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

        private List<string> saveList= new List<string>();
        private List<string> deleteList = new List<string>();

        [ObservableProperty]
        public bool _isModified = false;

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

        [ObservableProperty]
        public MediaPlayerService _mediaPlayerService;


        public FileSelectionViewModel(INavigationService navService)
        {
            Chapters = new Chapters();
            Navigation = navService;
            UsbService = new UsbService();
            UsbService.UpdateSelectedDrive += UpdateSelectedDrive;
            Application.Current.Dispatcher.Invoke(() => SelectedDrive = UsbService.UsbDrives.Last());
            SaveService = new SaveService();
            LoadService = new LoadService();
            MediaPlayerService = new MediaPlayerService();
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
                string fileExtension = Path.GetExtension(FileToAdd);
                List<string> validExtensions = new List<string> { ".mp4", ".mkv", ".mov", ".pdf", ".pptx" };
                if (String.IsNullOrEmpty(FileToAdd))
                {
                    return false;
                }
                else if (!validExtensions.Contains(fileExtension))
                {
                    FeedbackText = "Error: File extension not supported";
                    return false;
                }
                else if (Chapters.ChapterList.Any(chapter => chapter.Path == FileToAdd))
                {
                    FeedbackText = "Error: File already added";
                    return false;
                }
                else if(Chapters.ChapterList.Any(chapter => chapter.Title == Path.GetFileName(FileToAdd)))
                {
                    FeedbackText = "Error: File name already exists";
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
            get
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

        public void OnFilesDropped(string[] files)
        {
            FileToAdd = files[0];
            FeedbackText = "";
            if (FileValid)
            {
                Chapters.ChapterList.Add(new Chapter { Path = FileToAdd, Title = Path.GetFileName(FileToAdd) });
                FeedbackText = "File successfully added";
                saveList.Add(FileToAdd);
                IsModified = true;
            }
        }

        public void OnFileSelected(string[] files)
        {
            
            FeedbackText = "";
            foreach (var file in files){
                FileToAdd = file;
                if (FileValid)
                {
                    Chapters.ChapterList.Add(new Chapter { Path = FileToAdd, Title = Path.GetFileName(FileToAdd) });
                    FeedbackText = "File successfully added";
                    saveList.Add(FileToAdd);
                    IsModified = true;
                }
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
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                await Task.Run(() => SaveService.SaveMedia(saveList.ToArray()));
                await Task.Run(() => SaveService.DeleteMedia(deleteList.ToArray()));
                await Task.Run(() => Extension.Export.ExportFolders(SelectedDrive.Name,Chapters.ChapterList));
                UsbService.GetUsbInfo();
                UpdateSelectedDrive();
                IsModified = false;
                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// executes all methods needed to load the data into the application
        /// </summary>
        public async Task Import()
        { 
            Chapters.ChapterList=await Task.Run(() => LoadService.LoadMedia());
            await Task.Run(() => LoadService.LoadFonts());
        }

        [RelayCommand(CanExecute =nameof(ChapterChosen))]
        public void DeleteChapter()
        {
            deleteList.Add(SelectedChapter.Path);
            Chapters.ChapterList.Remove(SelectedChapter);
            FeedbackText = "File successfully removed";
            IsModified = true;
        }

        [RelayCommand(CanExecute = nameof(ChapterChosen))]
        public async Task OpenDefaultMediaPlayer()
        {
            //open Default Media Player using Service
            await Task.Run(() => MediaPlayerService.OpenMediaPlayer(SelectedChapter.Path));

        }
    }
}
