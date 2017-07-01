using System;
using System.ComponentModel;
using System.Windows.Input;
using BoilerController.Common.Utilities;
using Newtonsoft.Json;
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
        private string _onSince = "00:00:00";

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

        public string OnSince
        {
            get => _onSince;
            set
            {
                _onSince = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OnSince"));
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
                if (value != _isToggled)
                {
                    _isToggled = value;
                    SwitchStatus(value);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsToggled"));
                }
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

        #endregion

        #region Methods

        /// <summary>
        ///     Sends a command to the boiler to switch it's status manually.
        /// </summary>
        private async void SwitchStatus(bool state)
        {
            switch (state)
            {
                case true:
                    {
                        await HttpHandler.HttpRequestTask("setstate?dev=17&state=1");
                    }
                    break;
                case false:
                    {
                        await HttpHandler.HttpRequestTask("setstate?dev=17&state=0");
                    }
                    break;
            }
            UpdateProps();
        }

        /// <summary>
        /// Updates properties on startup
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
                var data = JsonConvert.DeserializeAnonymousType(content, new
                {
                    on_since = "",
                    state = ""
                });

                switch(data.state)
                {
                    case "On":
                        IsConnectedToServer = true;
                        _isToggled = true;
                        StatColor = Color.Green;
                        OnSince = DateTime.Parse(data.on_since).ToLongTimeString();
                        break;
                    case "Off":
                        IsConnectedToServer = true;
                        _isToggled = false;
                        StatColor = Color.Red;
                        break;
                    default:
                        IsConnectedToServer = false;
                        _isToggled = false;
                        break;

                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsToggled"));
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