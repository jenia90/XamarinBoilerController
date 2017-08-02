using System.ComponentModel;
using System.Windows.Input;
using BoilerController.Common.Utilities;
using Xamarin.Forms;

namespace BoilerController.ViewModels
{
    internal class SettingsPageViewModel : INotifyPropertyChanged
    {
        public SettingsPageViewModel()
        {
            var baseUrl = HttpHandler.BaseUrl.Split(':');
            ServerAddress = baseUrl[0];
            ServerPort = baseUrl[1];
        }
        private string _serverAddress;
        private string _serverPort;

        public string ServerAddress
        {
            get => _serverAddress;
            set
            {
                if (value == _serverAddress) return;
                _serverAddress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ServerAddress"));
            }
        }

        public string ServerPort
        {
            get => _serverPort;
            set
            {
                _serverPort = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ServerPort"));
            }
        }

        public ICommand SaveCommand => new Command( () =>
        {
            HttpHandler.BaseUrl = _serverAddress + ":" + _serverPort;
        });

        public event PropertyChangedEventHandler PropertyChanged;
    }
}