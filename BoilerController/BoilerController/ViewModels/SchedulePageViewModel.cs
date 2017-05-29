using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using BoilerController.Common.Utilities;
using BoilerController.Models;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace BoilerController.ViewModels
{
    internal class SchedulePageViewModel : INotifyPropertyChanged
    {
        private bool _isRefreshing;

        private ObservableCollection<Job> _jobs;

        private DateTime _onDate = DateTime.Now;

        private TimeSpan _onTime = DateTime.Now.TimeOfDay;

        private int _selectedDuration;
        private ObservableCollection<WeekDay> _days;

        public SchedulePageViewModel()
        {
            Durations = new ObservableCollection<string>
            {
                "15 mins",
                "30 mins",
                "45 mins",
                "1 hour",
                "1 hour 15 mins",
                "1 hour 30 mins",
                "1 hour 45 mins",
                "2 hours"
            };

            Days = new ObservableCollection<WeekDay>
            {
                new WeekDay {Day = "Sun", IsSelected = false},
                new WeekDay {Day = "Mon", IsSelected = false},
                new WeekDay {Day = "Tue", IsSelected = false},
                new WeekDay {Day = "Wed", IsSelected = false},
                new WeekDay {Day = "Thu", IsSelected = false},
                new WeekDay {Day = "Fri", IsSelected = false},
                new WeekDay {Day = "Sat", IsSelected = false}
            };
        }

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
                    App.CurrentPage.DisplayAlert("Error", "Start date cannot be in the past", "Dismiss");
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
                    App.CurrentPage.DisplayAlert("Error", "Start time cannot be in the past", "Dismiss");
                    OnTime = DateTime.Now.TimeOfDay;
                    return;
                }
                _onTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OnTime"));
            }
        }

        public ObservableCollection<string> Durations { get; }

        public ObservableCollection<WeekDay> Days
        {
            get => _days;
            private set
            {
                _days = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Days"));
            }
        }

        public int SelectedDuration
        {
            get => _selectedDuration;
            set
            {
                _selectedDuration = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedDuration"));
            }
        }

        public ICommand SetTimerCommand => new Command(SetTimer);
        public ICommand SetCronCommand => new Command(SetCronJob);
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
                    End = $@"{OnDate:yyyy-MM-dd} {OnTime.Add(new TimeSpan(0, SelectedDuration * 15 + 15, 0)):hh\:mm}",
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

        private async void SetCronJob()
        {
            try
            {
                IsRefreshing = true;

                var days = from weekDay in Days where weekDay.IsSelected select weekDay.Day;


                var job = JsonConvert.SerializeObject(new Job
                {
                    Pin = 17,
                    Start = $@"{OnDate:yyyy-MM-dd} {OnTime:hh\:mm}",
                    End = $@"{OnDate:yyyy-MM-dd} {OnTime.Add(new TimeSpan(0, SelectedDuration * 15 + 15, 0)):hh\:mm}",
                    Type = "cron",
                    DaysList = days
                });

                var response = await HttpHandler.HttpRequestTask("addcron", job, "POST");
                if (await response.Content.ReadAsStringAsync() == "OK")
                    GetTimes();
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