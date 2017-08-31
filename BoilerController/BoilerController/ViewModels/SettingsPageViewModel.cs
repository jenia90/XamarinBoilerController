using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using BoilerController.Common.Helpers;
using Xamarin.Forms;

namespace BoilerController.ViewModels
{
    internal class SettingsPageViewModel : INotifyPropertyChanged
    {
        private string _serverAddress;
        private string _serverPort;
        private string _username;
        private string _password;
        private bool _isPassChanged;

        public SettingsPageViewModel()
        {
            var baseUrl = NetworkHandler.BaseUrl.Split(':');
            ServerAddress = baseUrl[0];
            ServerPort = baseUrl[1];
            _username = Settings.Username;
            _password = Settings.Password;
        }

        public string ServerAddress
        {
            get => _serverAddress;
            set
            {
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

        public string Username  
        {
            get => _username;
            set
            {
                _username = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Username"));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (_password == value) return;
                _password = value;
                _isPassChanged = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Password"));
            }
        }
        
        public ICommand SaveCommand => new Command( () =>
        {
            NetworkHandler.BaseUrl = _serverAddress + ":" + _serverPort;
            Settings.ServerAddress = NetworkHandler.BaseUrl;
            Settings.Username = Username;

            if (_isPassChanged)
            {
                Settings.Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(Password));
                _isPassChanged = false;
            }
        });

        public event PropertyChangedEventHandler PropertyChanged;
    }
}