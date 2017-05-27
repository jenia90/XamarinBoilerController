using System;
using System.ComponentModel;
using System.Windows.Input;
using BoilerController.Utilities;
using Xamarin.Forms;

namespace BoilerController.ViewModels
{
    internal class MainPageViewModel : INotifyPropertyChanged
    {
        #region ctor

        public MainPageViewModel()
        {
            UpdateProps();
        }

        #endregion

        #region Fields

        private string _status = "Status unavailable";
        private Color _statColor;
        private Page _detail;
        private bool _isToggled;
        private bool _isConnectedToServer;

        #endregion

        #region Props

        public Page Detail
        {
            get => _detail;
            set
            {
                _detail = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Detail"));
            }
        }

        public string Status
        {
            get => _status;
            private set
            {
                _status = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
            }
        }

        public Color StatColor
        {
            get => _statColor;
            private set
            {
                _statColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StatColor"));
            }
        }

        public bool IsToggled
        {
            get => _isToggled;
            set
            {
                _isToggled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsToggled"));
            }
        }

        public bool IsConnectedToServer
        {
            get => _isConnectedToServer;
            set
            {
                _isConnectedToServer = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsConnectedToServer"));
            }
        }

        #endregion

        #region Commands

        public ICommand GetStatusCommand => new Command(UpdateProps);
        public ICommand SwitchCommand => new Command(SwitchStatus);

        #endregion

        #region Methods

        /// <summary>
        ///     Sends a command to the boiler to switch it's status manually.
        /// </summary>
        private async void SwitchStatus()
        {
            switch (IsToggled)
            {
                case true:
                    {
                        var response = await HttpHandler.HttpRequestTask("setstate?dev=17&state=1");
                        var content = await response.Content.ReadAsStringAsync();
                        if (content == "OK")
                        {
                            IsToggled = false;
                            StatColor = Color.Green;
                        }
                    }
                    break;
                case false:
                    {
                        var response = await HttpHandler.HttpRequestTask("setstate?dev=17&state=0");
                        var content = await response.Content.ReadAsStringAsync();
                        if (content == "OK")
                        {
                            IsToggled = true;
                            StatColor = Color.Red;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        ///     Updates properties on startup
        /// </summary>
        private async void UpdateProps()
        {
            try
            {
                var response = await HttpHandler.HttpRequestTask("getstate?dev=17");
                if (response == null)
                {
                    IsConnectedToServer = false;
                    return;
                }
                var content = await response.Content.ReadAsStringAsync();
                if (content == "On")
                {
                    IsConnectedToServer = true;
                    IsToggled = true;
                    StatColor = Color.Green;
                }
                else if (content == "Off")
                {
                    IsConnectedToServer = true;
                    IsToggled = false;
                    StatColor = Color.Red;
                }
                else
                {
                    IsToggled = false;
                    IsConnectedToServer = false;
                    //Status = content;
                    StatColor = Color.Blue;
                }
            }
            catch (Exception)
            {
                HttpHandler.DisplayMessage("Server Unreachable", "Unable to connect to server");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    #endregion
}