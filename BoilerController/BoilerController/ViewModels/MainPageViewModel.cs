using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BoilerController.ViewModels
{
    class MainPageViewModel : ContentPage
    {
        private readonly string _baseurl = "http://localhost:5000/";
        //private readonly string _baseurl = "http://192.168.1.178:5000/"; // uncomment for production
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
            var response = await HttpHandlerTask("getled17");
            if (response == "On")
            {
                Status = "On";
                StatColor = Color.Green;
            }
            else if (response == "Off")
            {
                Status = "Off";
                StatColor = Color.Red;
            }
            else
            {
                Status = response;
                StatColor = Color.Blue;
            }
        }

        public async Task<string> HttpHandlerTask(string request)
        {
            HttpResponseMessage response;

            using (var client = new HttpClient())
            {
                response = await client.GetAsync(_baseurl + request);
            }

            return await response.Content.ReadAsStringAsync();
        }
        
        public new event PropertyChangedEventHandler PropertyChanged;
    }
}
