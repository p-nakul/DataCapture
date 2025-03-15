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
        public List<List<string>> TableData { get; set; } = new();

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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
