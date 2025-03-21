using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCapture.Models
{
    public class HtmlFile : INotifyPropertyChanged
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public Dictionary<string, string> KeyValues { get; set; } = new();

        public List<HtmlSection> Sections { get; set; } = new();
        
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        private bool _isExtractionCompleted;
        public bool IsExtractionCompleted
        {
            get { return _isExtractionCompleted; }
            set
            {
                _isExtractionCompleted = value;
                OnPropertyChanged(nameof(IsExtractionCompleted));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class HtmlSection
    {
        public string Header { get; set; } = string.Empty;
        public string TextContent { get; set; } = "";  // Stores any text between h3 and table
        public List<List<string>> Table { get; set; } = new(); // Each table is a list of rows (List of List)
    }
}
