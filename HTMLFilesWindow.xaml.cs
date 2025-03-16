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
using System.ComponentModel;

namespace DataCapture
{
    /// <summary>
    /// Interaction logic for HTMLFilesWindow.xaml
    /// </summary>
    public partial class HTMLFilesWindow : Window
    {
        private BackgroundWorker _worker;
        private BackgroundWorker _excelWriter;
        private bool isExportingEverything = false;
        public ObservableCollection<HtmlFile> _html_files { get; set; } = new ObservableCollection<HtmlFile>();
        public HTMLFilesWindow(List<HtmlFile> html_files)
        {
            InitializeComponent();
            _html_files = new ObservableCollection<HtmlFile>(html_files);
            DataContext = this;
            _worker = new BackgroundWorker();
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
            _worker.WorkerSupportsCancellation = true;

            _excelWriter = new BackgroundWorker();
            _excelWriter.DoWork += _excelWriter_DoWork;
            _excelWriter.RunWorkerCompleted += _excelWriter_RunWorkerCompleted;
            _excelWriter.WorkerSupportsCancellation = true;

            exportEveryThing.IsEnabled = false;
          
        }

        private void _excelWriter_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("File exported Successfully!");
            try
            {
                Process.Start(new ProcessStartInfo(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "data_capture.xlsx")) { UseShellExecute = true });
            }
            catch (Exception ex) { 
            }

        }

        private void _excelWriter_DoWork(object? sender, DoWorkEventArgs e)
        {
            List<HtmlFile> htmlFiles = e.Argument as List<HtmlFile>;

            if (htmlFiles == null)
            {
                e.Cancel = true;
                return;
            }

            string excel_path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "data_capture.xlsx");
            Excelhelper.CreateExcelFile(excel_path);
            foreach (HtmlFile htmlFile in htmlFiles)
            {
                
                Excelhelper.WriteDictionaryToExcel(excel_path, htmlFile.FileName, htmlFile.KeyValues);
            }

        }

        private void _worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (isExportingEverything) {
                exportEveryThing.IsEnabled = true;
                isExportingEverything = false;
            }

            MessageBox.Show($"Extraction completed");
            //throw new NotImplementedException();
        }

        private void _worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            List<HtmlFile> htmlFiles = e.Argument as List<HtmlFile>;

            if (htmlFiles == null) { 
                e.Cancel = true;
                return;
            }

            foreach (HtmlFile htmlFile in htmlFiles) {
                HtmlProcessor processor = HtmlProcessorFactory.GetProcessor(htmlFile.FilePath);
                HtmlFile hf = processor.ExtractData(htmlFile);
                hf.IsExtractionCompleted = true;
            }


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
                if (_worker.IsBusy)
                {
                    MessageBox.Show($"Busy in processing! Please wait.", "Extract File");
                    return;
                }

                List<HtmlFile> htmlList = new List<HtmlFile>();
                htmlList.Add(file);
                _worker.RunWorkerAsync(htmlList);
                
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
            List<HtmlFile> selectedFiles = _html_files.Where(f => f.IsSelected).ToList();
            if (selectedFiles.Count == 0)
            {
                MessageBox.Show("No files selected!", "Warning");
                return;
            }

            _worker.RunWorkerAsync(selectedFiles);
            isExportingEverything = true;

            
        }

        // Export to excel
        private void ViewExtracted_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is HtmlFile file)
            {
                HtmlFile htmlFile = file as HtmlFile;
                List<HtmlFile> htmlList = new List<HtmlFile>();
                htmlList.Add(htmlFile);

                _excelWriter.RunWorkerAsync(htmlList);
            }
        }

        private void exportEveryThing_Click(object sender, RoutedEventArgs e)
        {
            List<HtmlFile> selectedFiles = _html_files.Where(f => f.IsSelected).ToList();
            _excelWriter.RunWorkerAsync(selectedFiles);

        }
    }
}
