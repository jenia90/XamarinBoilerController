using System.ComponentModel;
using System.Windows.Input;
using BoilerController.Utilities;
using Xamarin.Forms;

namespace BoilerController.ViewModels
{
    internal class SettingsPageViewModel : INotifyPropertyChanged
    {
        private string _serverAddress;

        public string ServerAddress
        {
            get => "http://" + _serverAddress + "/api/";
            set
            {
                if (value == _serverAddress) return;
                _serverAddress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ServerAddress"));
            }
        }

        public ICommand SaveCommand => new Command(
            () => { HttpHandler.BaseUrl = "http://" + _serverAddress + "/api/"; });

        public event PropertyChangedEventHandler PropertyChanged;
    }
}