using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Control
{
    public class Appx : INotifyPropertyChanged
    {
        private string name;
        private string fullName;
        private string installLocation;
        private bool onlineProvisioned;
        private bool remove;

        [JsonPropertyName("Name")]
        public string Name
        {
            get => OnlineProvisioned ? "(Online) " + name : name;
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        [JsonPropertyName("FullName")]
        public string FullName
        {
            get => fullName;
            set
            {
                fullName = value;
                OnPropertyChanged("FullName");
            }
        }

        [JsonPropertyName("InstallLocation")]
        public string InstallLocation
        {
            get => installLocation;
            set
            {
                installLocation = value;
                OnPropertyChanged("InstallLocation");
            }
        }

        public bool Remove
        {
            get => remove;
            set
            {
                remove = value;
                OnPropertyChanged("Remove");
            }
        }

        [JsonPropertyName("OnlineProvisioned")]
        public bool OnlineProvisioned
        {
            get => onlineProvisioned;
            set
            {
                onlineProvisioned = value;
                OnPropertyChanged("OnlineProvisioned");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}