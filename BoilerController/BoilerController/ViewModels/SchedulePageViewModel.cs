using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using BoilerController.Common.Helpers;
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
#region Init collections
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
#endregion

            GetTimes();
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

        public ICommand SetTimerCommand => new Command<string>(SetTimer);
        public ICommand GetTimesCommand => new Command(GetTimes);
        public ICommand DeleteCommand => new Command<int>(RemoveItem);

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Adds a scheduled activation
        /// </summary>
        /// <param name="type">type of schedule: </param>
        private async void SetTimer(string type)
        {
            try
            {
                // Set the listview to refreshing state
                IsRefreshing = true;

                // Get list of selected days
                List<string> days = new List<string>(from weekDay in Days where weekDay.IsSelected select weekDay.Day);

                // Check if the command is daily schedule in which case check that some days were selected
                if (type == "addcron" && days.Count == 0)
                {
                    IsRefreshing = false;
                    await App.CurrentPage.DisplayAlert("Error Occured", "No days for recurring schedule selected.", "Dismiss");
                    return;
                }

                // Serealize a new Job object
                var job = JsonConvert.SerializeObject(new Job
                {
                    Pin = 17,
                    Start = $@"{OnDate:yyyy-MM-dd} {OnTime:hh\:mm}",
                    End = DateTime.Parse($@"{OnDate:yyyy-MM-dd} {OnTime:hh\:mm}").Add(
                        new TimeSpan(0, SelectedDuration * 15 + 15, 0)).ToString("yyyy-MM-dd HH:mm"),
                    Type = type == "settime" ? "datetime" : "cron",
                    DaysList = days
                });

                // Send the request to the server and in case of success update the listview
                var response = await NetworkHandler.GetResponseTask(type, job, "POST");
                if (await response.Content.ReadAsStringAsync() == "OK")
                {
                    GetTimes();
                    foreach (var day in Days)
                        if (days.Contains(day.Day))
                            day.IsSelected = false;
                }
            }
            catch (Exception e)
            {
                await App.CurrentPage.DisplayAlert("Error Occured", e.Message, "Dismiss");
            }
            finally { IsRefreshing = false; }
        }

        /// <summary>
        /// Populates the list of scheduled jobs
        /// </summary>
        private async void GetTimes()
        {
            try
            {
                // If already refreshing no need to set the state again
                if (!IsRefreshing)
                    IsRefreshing = true;

                // Get the list of jobs from the server
                var response = await NetworkHandler.GetResponseTask("gettimes");
                var job = await response.Content.ReadAsStringAsync();

                // Deserialize the jobs list and updat the Jobs collection
                Jobs = JsonConvert.DeserializeObject<ObservableCollection<Job>>(job);
                
                IsRefreshing = false;
            }
            catch (Exception e)
            {
                await App.Current.MainPage.DisplayAlert("Error Occured", e.Message, "Dismiss");
            }
            finally { IsRefreshing = false; }
        }

        // Removes the selected item
        private async void RemoveItem(int id)
        {
            try
            {
                var response = await NetworkHandler.GetResponseTask("remove?id=" + id, method: "DELETE");
                if (await response.Content.ReadAsStringAsync() == "OK")
                    GetTimes();
            }
            catch (Exception e)
            {
                await App.CurrentPage.DisplayAlert("Error Occured", e.Message, "Dismiss");
            }
        }
    }
}