using System;
using System.Collections.Generic;
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
        private string _activityString = "";
        private int _durationIndex = 7;

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

        public string ActivityString
        {
            get => _activityString;
            set
            {
                _activityString = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ActivityString"));
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

        public int DurationIndex
        {
            get => _durationIndex;
            set
            {
                _durationIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DurationIndex"));
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
            if (DurationIndex == 7)
            {
                await App.Boiler.SetStateTask(state);
            }
            else
            {
                await App.Boiler.SetScheduledJobTask(DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                    DateTime.Now.AddMinutes(DurationIndex * 15 + 15).ToString("yyyy-MM-dd HH:mm"), "datetime", new List<string>());
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
                var data = await App.Boiler.GetCurrentStateTask();
                var activityTime = data.OnSince.Length > 0 ? 
                    DateTime.Parse(data.OnSince).ToString("HH:mm") : "Never";

                switch (data.State)
                {
                    case "On":
                        StatColor = Color.Green;
                        IsConnectedToServer = true;
                        IsToggled = true;
                        ActivityString = "On Since:\n" + activityTime;
                        break;
                    case "Off":
                        StatColor = Color.Red;
                        IsConnectedToServer = true;
                        IsToggled = false;
                        ActivityString = "Last Active:\n" + activityTime;
                        break;
                    default:
                        IsConnectedToServer = false;
                        IsToggled = false;
                        StatColor = Color.DarkGray;
                        ActivityString = "Unable to connect";
                        throw new Exception("Server Unreachable");

                }
            }
            catch (Exception)
            {
                var res = await Application.Current.MainPage.DisplayAlert("Error Occured",
                    "Unable to get status from the server. Makes sure your settings are correct.\n" +
                    "Do you want to proceed to settings page?", "Ok", "Dismiss");
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