using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using BoilerController.Models;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace BoilerController.ViewModels
{
    class MainPageViewModel : INotifyPropertyChanged
    {
        #region Fields

        //private readonly string _baseurl = "http://localhost:5000/";
        private readonly string _baseurl = "http://192.168.1.178:5000/"; // uncomment for production
        private string _status = "Status unavailable";
        private Color _statColor;
        private DateTime _onDate = DateTime.Now, _offDate = DateTime.Now;
        private TimeSpan _onTime = DateTime.Now.TimeOfDay,
            _offTime = DateTime.Now.AddMinutes(45).TimeOfDay;

        private ObservableCollection<Job> _jobs;

        #endregion

        #region ctor

        public MainPageViewModel()
        {
            UpdateProps();
        } 
        #endregion

        #region Props

        public ObservableCollection<Job> Jobs
        {
            get => _jobs;
            set
            {
                _jobs = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Jobs"));
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

        public DateTime OnDate
        {
            get => _onDate;
            set
            {
                if (value <= DateTime.Now)
                {
                    DisplayMessage("Error in start date", "Start date cannot be in the past");
                    OnDate = DateTime.Now;
                    return;
                }
                _onDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OnDate"));
            }
        }

        public TimeSpan OnTime
        {
            get => _onTime;
            set
            {
                if (value <= DateTime.Today.TimeOfDay)
                {
                    DisplayMessage("Error in start time", "Start time cannot be in the past");
                    OnTime = DateTime.Now.TimeOfDay;
                    return;
                }
                _onTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OnTime"));
            }
        }

        public DateTime OffDate
        {
            get => _offDate;
            set
            {
                if (value < OnDate)
                {
                    DisplayMessage("Error in end time", "End date cannot be before start time");
                    OffDate = DateTime.Now;
                    return;
                }
                _offDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OffDate"));
            }
        }

        public TimeSpan OffTime
        {
            get => _offTime;
            set
            {
                if (value < OnTime)
                {
                    DisplayMessage("Error in end time", "End time cannot be before start time");
                    OffTime = DateTime.Now.TimeOfDay;
                    return;
                }
                _offTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OffTime"));
            }
        }
        #endregion

        #region Commands

        public RelayCommand SwitchCommand => new RelayCommand(SwitchStatus);
        public RelayCommand SetTimerCommand => new RelayCommand(SetTimer);
        public RelayCommand GetTimesCommand => new RelayCommand(GetTimes);

        #endregion

        #region Methods

        /// <summary>
        /// Sends a command to the boiler to switch it's status manually.
        /// </summary>
        private async void SwitchStatus()
        {
            if (Status == "Off")
            {
                var response = await HttpRequestTask("setled17/1");
                var content = await response.Content.ReadAsStringAsync();
                if (content == "OK")
                {
                    Status = "On";
                    StatColor = Color.Green;
                }
            }
            else if (Status == "On")
            {
                var response = await HttpRequestTask("setled17/0");
                var content = await response.Content.ReadAsStringAsync();
                if (content == "OK")
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
                $@"settime?dev=17&ontime={OnTime:hh\:mm}&ondate={OnDate:yyyy-MM-dd}"+
                $@"&offtime={OffTime:hh\:mm}&offdate={OffDate:yyyy-MM-dd}";


            await HttpRequestTask(request);
        }

        private async void GetTimes()
        {
            //TODO: Implement!
            var response = await HttpRequestTask("gettimes");
            var job = await response.Content.ReadAsStringAsync();
            Jobs = JsonConvert.DeserializeObject<ObservableCollection<Job>>(job);
        }

        /// <summary>
        /// Updates properties on startup
        /// </summary>
        private async void UpdateProps()
        {
            var response = await HttpRequestTask("getled17");
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

        /// <summary>
        /// Sends formated request to the boiler server
        /// </summary>
        /// <param name="request" type="HttpResponseMessage">Request string to send</param>
        /// <returns>Return status from the server</returns>
        private async Task<HttpResponseMessage> HttpRequestTask(string request)
        {
            HttpResponseMessage response;

            using (var client = new HttpClient())
            {
                try
                {
                    response = await client.GetAsync(_baseurl + request);
                }
                catch (HttpRequestException e)
                {
                    DisplayMessage("Server Unreachable", "Unable to connect to server");
                    return null;
                }
            }

            return response;
        }

        private async void DisplayMessage(string title, string message, string cancel = "OK")
        {
            await App.Current.MainPage.DisplayAlert(title, message, cancel);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    } 
    #endregion
}
