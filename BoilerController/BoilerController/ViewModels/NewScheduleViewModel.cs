using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Windows.Input;
using BoilerController.Common.Helpers;
using BoilerController.Models;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace BoilerController.ViewModels
{
    class NewScheduleViewModel : INotifyPropertyChanged
    {
        #region Member Fields

        private DateTime _onDate;
        private TimeSpan _onTime;
        private int _selectedDuration;
        private ObservableCollection<WeekDay> _days;

        #endregion

        #region ctor

        public NewScheduleViewModel()
        {
            _onTime = DateTime.Now.TimeOfDay;
            _onDate = DateTime.Now.Date;

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

        #endregion

        #region Props
        public DateTime OnDate
        {
            get => _onDate;
            set
            {
                if (value <= DateTime.Now.Date)
                {
                    Application.Current.MainPage.DisplayAlert("Error", "Start date cannot be in the past", "Dismiss");
                    OnDate = DateTime.Now.Date;
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
                    Application.Current.MainPage.DisplayAlert("Error", "Start time cannot be in the past", "Dismiss");
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
        #endregion

        #region Commands

        public ICommand SetTimerCommand => new Command<string>(SetTimer);
        public ICommand DiscardCommand => new Command(NavigateBack);

        #endregion

        #region Methods
        
        /// <summary>
        /// Adds a scheduled activation
        /// </summary>
        /// <param name="type">type of schedule: </param>
        private async void SetTimer(string type)
        {
            try
            {

                // Get list of selected days
                List<string> days = new List<string>(from weekDay in Days where weekDay.IsSelected select weekDay.Day);

                // Check if the command is daily schedule in which case check that some days were selected
                if (type == "addcron" && days.Count == 0)
                {
                    await App.Current.MainPage.DisplayAlert("Error Occured", "No days for recurring schedule selected.",
                        "Dismiss");
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
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Remote operation failed.");
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error Occured", e.Message, "Dismiss");
            }
            finally
            {
                NavigateBack();
            }
        }

        private async void NavigateBack()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
