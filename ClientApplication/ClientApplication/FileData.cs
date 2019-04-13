using System.ComponentModel;

namespace ClientApplication
{
    public class FileData : INotifyPropertyChanged
    {
        private string _Time = string.Empty;
        private string _Note1 = string.Empty;
        private string _Note2 = string.Empty;
        private string _Name = string.Empty;
        private string _Value = string.Empty;

        public string Time
        {
            get
            {
                return _Time;
            }
            set
            {
                _Time = value;
                NotifyChange("Time");
            }
        }

        public string Note1
        {
            get
            {
                return _Note1;
            }
            set
            {
                _Note1 = value;
                NotifyChange("Note1");
            }
        }

        public string Note2
        {
            get
            {
                return _Note2;
            }
            set
            {
                _Note2 = value;
                NotifyChange("Note2");
            }
        }

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                NotifyChange("Name");
            }
        }

        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                NotifyChange("Value");
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