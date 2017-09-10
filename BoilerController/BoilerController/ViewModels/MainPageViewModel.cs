using System;
using System.ComponentModel;
using System.Windows.Input;
using BoilerController.Views;
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
            await App.Boiler.SetStateTask(state);
            
            UpdateProps();
        }

        /// <summary>
        /// Updates properties on startup
        /// </summary>
        private async void UpdateProps()
        {
            try
            {
                var data = await App.Boiler.GetCurrentStateTask();

                switch(data.State)
                {
                    case "On":
                        StatColor = Color.Green;
                        IsConnectedToServer = true;
                        IsToggled = true;
                        OnSince = DateTime.Parse(data.OnSince).ToString("HH:mm");
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
                var res = await Application.Current.MainPage.DisplayAlert("Error Occured",
                    "Unable to get status from the server. Makes sure your settings are correct.\nDo you want to proceed to settings page?", "Ok", "Dismiss");
                if (res)
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new NavigationPage(new SettingsPage()));
                }
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
    }

}