using DataCapture.HTMLParser;
using DataCapture.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace DataCapture
{
    /// <summary>
    /// Interaction logic for HTMLFilesWindow.xaml
    /// </summary>
    public partial class HTMLFilesWindow : Window
    {
        public ObservableCollection<HtmlFile> _html_files { get; set; } = new ObservableCollection<HtmlFile>();
        public HTMLFilesWindow(List<HtmlFile> html_files)
        {
            InitializeComponent();
            _html_files = new ObservableCollection<HtmlFile>(html_files);
            DataContext = this;
        }

        private void ViewFile(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is HtmlFile file)
            {
                try
                {
                    Process.Start(new ProcessStartInfo(file.FilePath) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExtractFile(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is HtmlFile file)
            {
                HtmlProcessor processor = HtmlProcessorFactory.GetProcessor(file.FilePath);
                HtmlFile hf = processor.ExtractData(file);
                MessageBox.Show($"Extracting data from: {file.FileName}", "Extract File");
            }
        }

        private void SelectAllChecked(object sender, RoutedEventArgs e)
        {
            bool isChecked = (sender as CheckBox).IsChecked ?? false;
            foreach (var file in _html_files)
            {
                file.IsSelected = isChecked;
            }
        }

        private void extractAllBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedFiles = _html_files.Where(f => f.IsSelected).ToList();
            if (selectedFiles.Count == 0)
            {
                MessageBox.Show("No files selected!", "Warning");
                return;
            }

            foreach (var file in selectedFiles)
            {
                MessageBox.Show($"Extracting: {file.FileName}", "Extracting");
            }
        }
    }
}
