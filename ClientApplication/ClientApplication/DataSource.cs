using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ClientApplication
{
    public class DataSource : INotifyPropertyChanged
    {
        private string _FileName = "Browse...";
        private ObservableCollection<FileData> _EvenDataSource = new ObservableCollection<FileData>();
        private ObservableCollection<FileData> _OddDataSource = new ObservableCollection<FileData>();

        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                _FileName = value;
                NotifyChange("FileName");

            }
        }

        public ObservableCollection<FileData> EvenDataSource
        {
            get
            {
                return _EvenDataSource;
            }
            set
            {
                _EvenDataSource = value;
                NotifyChange("EvenDataSource");

            }
        }

        public ObservableCollection<FileData> OddDataSource
        {
            get
            {
                return _OddDataSource;
            }
            set
            {
                _OddDataSource = value;
                NotifyChange("OddDataSource");

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}