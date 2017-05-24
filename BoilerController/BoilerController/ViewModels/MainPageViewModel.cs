using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BoilerController.Models;
using Newtonsoft.Json;
using Xamarin.Forms;
using BoilerController.Utilities;
using BoilerController.Views;
using Xamarin.Forms.Xaml;

namespace BoilerController.ViewModels
{
    class MainPageViewModel : INotifyPropertyChanged
    {
        #region Fields

        private string _status = "Status unavailable";
        private Color _statColor;
        private Page _detail;

        #endregion

        #region ctor

        public MainPageViewModel()
        {
            UpdateProps();
        } 
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

        
        #endregion

        #region Commands

        public ICommand GetStatusCommand => new Command(UpdateProps);
        public ICommand SwitchCommand => new Command(SwitchStatus);

        #endregion

        #region Methods


        /// <summary>
        /// Sends a command to the boiler to switch it's status manually.
        /// </summary>
        private async void SwitchStatus()
        {
            switch (Status)
            {
                case "Off":
                {
                    var response = await HttpHandler.HttpRequestTask("setstate?dev=17&state=1");
                    var content = await response.Content.ReadAsStringAsync();
                    if (content == "OK")
                    {
                        Status = "On";
                        StatColor = Color.Green;
                    }
                }
                    break;
                case "On":
                {
                    var response = await HttpHandler.HttpRequestTask("setstate?dev=17&state=0");
                    var content = await response.Content.ReadAsStringAsync();
                    if (content == "OK")
                    {
                        Status = "Off";
                        StatColor = Color.Red;
                    }
                }
                    break;
            }
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
                    return;
                }
                var content = await response.Content.ReadAsStringAsync();
                if (content == "On")
                {
                    Status = "On";
                    StatColor = Color.Green;
                }
                else if (content == "Off")
                {
                    Status = "Off";
                    StatColor = Color.Red;
                }
                else
                {
                    Status = content;
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
