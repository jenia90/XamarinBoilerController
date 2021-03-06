﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using BoilerController.Common.Models;
using Xamarin.Forms;

namespace BoilerController.ViewModels
{
    internal class NewScheduleViewModel : INotifyPropertyChanged
    {
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

            _days = new ObservableCollection<WeekDay>
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

        public event PropertyChangedEventHandler PropertyChanged;

        #region Member Fields

        private DateTime _onDate;
        private TimeSpan _onTime;
        private int _selectedDuration;
        private ObservableCollection<WeekDay> _days;

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
        ///     Adds a scheduled activation
        /// </summary>
        /// <param name="type">type of schedule: </param>
        private async void SetTimer(string type)
        {
            try
            {
                var start = $@"{OnDate:yyyy-MM-dd} {OnTime:hh\:mm}";
                var end = DateTime.Parse($@"{OnDate:yyyy-MM-dd} {OnTime:hh\:mm}").Add(
                    new TimeSpan(0, SelectedDuration * 15 + 15, 0)).ToString("yyyy-MM-dd HH:mm");
                // Get list of selected days
                var days = new List<string>(from weekDay in Days where weekDay.IsSelected select weekDay.Day);

                // Check if the command is daily schedule in which case check that some days were selected
                if (type == "addcron" && days.Count == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Error Occured",
                        "No days for recurring schedule selected.",
                        "Dismiss");
                    return;
                }

                await App.Boiler.SetScheduledJobTask(start, end, type == "settime" ? "datetime" : "cron", days);
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

        /// <summary>
        /// Navigates to the previous page.
        /// </summary>
        private async void NavigateBack() => await Application.Current.MainPage.Navigation.PopAsync();

        #endregion
    }
}