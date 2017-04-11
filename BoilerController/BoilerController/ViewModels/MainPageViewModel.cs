using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using BoilerController.Droid.Annotations;
using Xamarin.Forms;

namespace BoilerController.ViewModels
{
    class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly string _baseurl = "http://192.168.1.178:5000/";
        private string _status;
        private Color _statColor;

        public MainPageViewModel()
        {
            Status = "Off";
            StatColor = Color.Red;
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

        public RelayCommand SwitchCommand => new RelayCommand(SwitchStatus);

        private async void SwitchStatus()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    if (Status == "Off")
                    {
                        Status = "On";
                        StatColor = Color.Green;
                        var response = await client.GetAsync(_baseurl + "setled/1");
                    }
                    else if (Status == "On")
                    {
                        Status = "Off";
                        StatColor = Color.Red;
                        var response = await client.GetAsync(_baseurl + "setled/0");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }


        #region INPC
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
