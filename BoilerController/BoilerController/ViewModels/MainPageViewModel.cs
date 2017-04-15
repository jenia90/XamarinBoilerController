using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BoilerController.ViewModels
{
    class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly string _baseurl = "http://localhost:5000/";
        //private readonly string _baseurl = "http://192.168.1.178:5000/"; // uncomment for production
        private string _status = "Status unavailable";
        private Color _statColor;
        private DateTime _onDate = DateTime.Now, _offDate = DateTime.Now;
        private TimeSpan _onTime = DateTime.Now.TimeOfDay,
            _offTime = DateTime.Now.AddMinutes(45).TimeOfDay;

        public MainPageViewModel()
        {
            UpdateProps();
        }

        #region Props

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

        public DateTime OnDate
        {
            get => _onDate;
            set
            {
                _onDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OnDate"));
            }
        }

        public TimeSpan OnTime
        {
            get => _onTime;
            set
            {
                _onTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OnTime"));
            }
        }

        public DateTime OffDate
        {
            get => _offDate;
            set
            {
                _offDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OffDate"));
            }
        }

        public TimeSpan OffTime
        {
            get => _offTime;
            set
            {
                _offTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OffTime"));
            }
        }

        public RelayCommand SwitchCommand => new RelayCommand(SwitchStatus);
        public RelayCommand SetTimerCommand => new RelayCommand(SetTimer);

        #endregion

        /// <summary>
        /// Sends a command to the boiler to switch it's status manually.
        /// </summary>
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

        /// <summary>
        /// Sends a schedule command to the boiler
        /// </summary>
        private async void SetTimer()
        {
            var request =
                $@"settime?dev=17&ontime={OnTime:hh\:mm}&ondate={OnDate:yyyy-MM-dd}&offtime={OffTime:hh\:mm}&offdate={OffDate:yyyy-MM-dd}";


            await HttpHandlerTask(request);
        }

        /// <summary>
        /// Updates properties on startup
        /// </summary>
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

        /// <summary>
        /// Sends formated request to the boiler server
        /// </summary>
        /// <param name="request">Request string to send</param>
        /// <returns>Return status from the server</returns>
        public async Task<string> HttpHandlerTask(string request)
        {
            HttpResponseMessage response;

            using (var client = new HttpClient())
            {
                response = await client.GetAsync(_baseurl + request);
            }

            return await response.Content.ReadAsStringAsync();
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
