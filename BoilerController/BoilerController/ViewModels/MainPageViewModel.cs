using System;
using System.ComponentModel;
using System.Windows.Input;
using BoilerController.Common.Helpers;
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

        private Color _statColor;
        private Page _detail;
        private bool _isToggled;
        private bool _isConnectedToServer;
        private string _onSince = "";

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
                        await NetworkHandler.GetResponseTask("setstate?dev=17&state=1");
                    }
                    break;
                case false:
                    {
                        await NetworkHandler.GetResponseTask("setstate?dev=17&state=0");
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
                var response = await NetworkHandler.GetResponseTask("getstate?dev=17");
                if (response == null || !response.IsSuccessStatusCode)
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
                        StatColor = Color.Green;
                        IsConnectedToServer = true;
                        IsToggled = true;
                        OnSince = DateTime.Parse(data.on_since).ToString("HH:mm");
                        break;
                    case "Off":
                        StatColor = Color.Red;
                        IsConnectedToServer = true;
                        IsToggled = false;
                        OnSince = "";
                        break;
                    default:
                        IsConnectedToServer = false;
                        IsToggled = false;
                        StatColor = Color.DarkGray;
                        throw new Exception("Server Unreachable");

                }
            }
            catch (Exception e)
            {
                await App.Current.MainPage.Navigation.PushAsync(new Views.SettingsPage());
                //await App.CurrentPage.DisplayAlert("Server Unreachable", "Unable to connect to server", "Dismiss");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    #endregion
}