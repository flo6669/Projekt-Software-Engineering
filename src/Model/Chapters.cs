using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Effektive_Praesentationen.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using Effektive_Praesentationen.ViewModel;

namespace Effektive_Praesentationen.Model
{
    public partial class Chapters : ObservableObject
    {
        public Chapters()
        {
            ChapterList = new List<Chapter>();
        }

        [ObservableProperty]
        private string? defaultChapter;

        [ObservableProperty]
        private List<Chapter>? chapterList;

        [ObservableProperty]
        private string? databasePath;
    }
}