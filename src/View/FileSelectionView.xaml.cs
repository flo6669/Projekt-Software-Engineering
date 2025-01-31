﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Effektive_Praesentationen.View
{
    /// <summary>
    /// Interaktionslogik für FileSelectionView.xaml
    /// </summary>
    public partial class FileSelectionView : UserControl
    {
        public FileSelectionView()
        {
            InitializeComponent();
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(sender is ListBox listBox)
            {
                if(listBox.SelectedItem != null)
                {
                    var viewModel = DataContext as ViewModel.FileSelectionViewModel;
                    viewModel?.OpenDefaultMediaPlayerCommand.Execute(null);
                }
            }
        }
    }
}
