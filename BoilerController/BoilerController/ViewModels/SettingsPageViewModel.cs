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
            ServerAddress = HttpHandler.BaseUrl;
        }
        private string _serverAddress;

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

        public ICommand SaveCommand => new Command(() => 
        { HttpHandler.BaseUrl = _serverAddress; });

        public event PropertyChangedEventHandler PropertyChanged;
    }
}