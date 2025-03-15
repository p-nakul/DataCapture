using Microsoft.Win32;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataCapture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker _worker;
        private string _selected_zip_file = "";

        private int _number_of_files_found = 0;
        
        private List<Models.HtmlFile> _html_files = new List<Models.HtmlFile>();
        public MainWindow()
        {
            InitializeComponent();
            processingBtn.IsEnabled = false;
            _worker = new BackgroundWorker();
            _worker.DoWork += Worker_DoWork;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            _worker.ProgressChanged += Worker_ProgressChanged;
            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;
            processFilesStatus.Text = "";
        }

        private void Worker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if(e.ProgressPercentage == -1)
            {
                // unzip main zip failed
            }

            if (e.ProgressPercentage == -2)
            {
                // unzip nested zip failed
            }

            if (e.ProgressPercentage == 1)
            {
                fileProcessingBar.Value = 10;
            }

            if (e.ProgressPercentage == 2)
            {
                // unzip nested zip failed
                fileProcessingBar.Value = 40;
            }

            if (e.ProgressPercentage == 3)
            {
                // unzip nested zip failed
                fileProcessingBar.Value = 100;
                processFilesStatus.Text = $"Found {_number_of_files_found} html files";
            }
        }

        private void Worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        private void Worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            string tempFolderPath = Fileutils.ExtractZipToTemp(_selected_zip_file);
            if (tempFolderPath.Equals("")) {
                _worker.ReportProgress(-1);
                return;
            }

            _worker.ReportProgress(1);

            if (!Fileutils.ExtractNestedZips(tempFolderPath))
            {
                _worker.ReportProgress(-2);
                return;
            }

            _worker.ReportProgress(2);

            List<string> files = Fileutils.GetAllFiles(tempFolderPath);
            _number_of_files_found = files.Count;

            _html_files.Clear();
            foreach (var file in files)
            {
                _html_files.Add(new Models.HtmlFile { FileName = System.IO.Path.GetFileName(file), FilePath = file });
            }

            _worker.ReportProgress(3);
        }

        private void uploadFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog();

            DialogResult dr = fileDialog.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.OK) {
                uploadIcon.Visibility = Visibility.Collapsed;
                string filename = System.IO.Path.GetFileName(fileDialog.FileName);
                uploadedZipName.Text = filename;
                uploadedZipName.Visibility = Visibility.Visible;
                processingBtn.IsEnabled = true;
                _selected_zip_file = fileDialog.FileName;
            }
            else
            {

            }
        }

        

        private void processingBtn_Click(object sender, RoutedEventArgs e)
        {
            
            if (!_worker.IsBusy) {
                processFilesStatus.Text = "";
                _worker.RunWorkerAsync();
            }
            else
            {
                _worker.CancelAsync();
            }
        }

        private void ViewHtmlFiles_Click(object sender, RoutedEventArgs e)
        {
            HTMLFilesWindow hfw = new HTMLFilesWindow(_html_files);
            hfw.Show();
        }
    }
}