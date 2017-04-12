using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
            UpdateProps();
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

        public DateTime DateTime => DateTime.Now;

        public RelayCommand SwitchCommand => new RelayCommand(SwitchStatus);

        private async void SwitchStatus()
        {
            if (Status == "Off")
            {
                if (await HttpHandlerTask("setled17/1") == "OK")
                {
                    Status = "On";
                    StatColor = Color.Green;
                }
            }
            else if (Status == "On")
            {
                if (await HttpHandlerTask("setled17/0") == "OK")
                {
                    Status = "Off";
                    StatColor = Color.Red;
                }
            }
        }

        private async void SetTimer()
        {
            
        }

        private async void UpdateProps()
        {
            if (await HttpHandlerTask("getled17") == "On")
            {
                Status = "On";
                StatColor = Color.Green;
            }
            else if (await HttpHandlerTask("getled17") == "Off")
            {
                Status = "Off";
                StatColor = Color.Red;
            }
        }

        private async Task<string> HttpHandlerTask(string request)
        {
            HttpResponseMessage response;

            using (var client = new HttpClient())
            {
                try
                {
                    response = await client.GetAsync(_baseurl + request);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            //TODO: Fix this shit!!!! it doesn't return the message from the server!
            return response.ReasonPhrase;
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
