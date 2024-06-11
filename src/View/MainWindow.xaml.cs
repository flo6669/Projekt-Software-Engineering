using Effektive_Praesentationen.ViewModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Effektive_Praesentationen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;
            FileSelectionViewModel fileSelectionViewModel = (FileSelectionViewModel)viewModel.Navigation.CurrentViewModel;
            if (fileSelectionViewModel != null && fileSelectionViewModel.IsModified)
            {
                MessageBoxResult result = MessageBox.Show("There have been changes since the last export. Do you really want to close the window?", "Unsaved changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true; // Abbrechen des Schließens
                }
            }
        }
    }
}