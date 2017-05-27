using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using BoilerController.Models;
using BoilerController.Utilities;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace BoilerController.ViewModels
{
    internal class SchedulePageViewModel : INotifyPropertyChanged
    {
        private bool _isRefreshing;

        private ObservableCollection<Job> _jobs;

        private DateTime _onDate = DateTime.Now, _offDate = DateTime.Now;

        private TimeSpan _onTime = DateTime.Now.TimeOfDay,
            _offTime = DateTime.Now.AddMinutes(45).TimeOfDay;

        public ObservableCollection<Job> Jobs
        {
            get => _jobs;
            set
            {
                _jobs = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Jobs"));
            }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRefreshing"));
            }
        }

        public DateTime OnDate
        {
            get => _onDate;
            set
            {
                if (value <= DateTime.Now)
                {
                    HttpHandler.DisplayMessage("Error in start date", "Start date cannot be in the past");
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
                    HttpHandler.DisplayMessage("Error in start time", "Start time cannot be in the past");
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
                    HttpHandler.DisplayMessage("Error in end time", "End date cannot be before start time");
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
                    //DisplayMessage("Error in end time", "End time cannot be before start time");
                    OffTime = DateTime.Now.TimeOfDay;
                    return;
                }
                _offTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OffTime"));
            }
        }

        public ICommand SetTimerCommand => new Command(SetTimer);
        public ICommand GetTimesCommand => new Command(GetTimes);
        public ICommand DeleteCommand => new Command<int>(RemoveItem);

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Sends a schedule command to the boiler
        /// </summary>
        private async void SetTimer()
        {
            try
            {
                IsRefreshing = true;
                var job = JsonConvert.SerializeObject(new Job
                {
                    Pin = 17,
                    Start = $@"{OnDate:yyyy-MM-dd} {OnTime:hh\:mm}",
                    End = $@"{OffDate:yyyy-MM-dd} {OffTime:hh\:mm}",
                    Type = "datetime"
                });


                var response = await HttpHandler.HttpRequestTask("settime", job, "POST");
                if (await response.Content.ReadAsStringAsync() == "OK")
                    GetTimes();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void SetCronJob()
        {
            try
            {
                IsRefreshing = true;

                var job = JsonConvert.SerializeObject(new Job());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async void GetTimes()
        {
            try
            {
                var response = await HttpHandler.HttpRequestTask("gettimes");
                var job = await response.Content.ReadAsStringAsync();
                Jobs = JsonConvert.DeserializeObject<ObservableCollection<Job>>(job);

                IsRefreshing = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async void RemoveItem(int id)
        {
            try
            {
                var response = await HttpHandler.HttpRequestTask("remove?id=" + id);
                if (await response.Content.ReadAsStringAsync() == "OK")
                    GetTimes();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}