using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using BoilerController.Common.Helpers;
using BoilerController.Models;
using BoilerController.Views;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace BoilerController.ViewModels
{
    internal class SchedulePageViewModel : INotifyPropertyChanged
    {
        #region Member Fields

        private bool _isRefreshing;
        private ObservableCollection<Job> _jobs;

        #endregion

        #region Ctor

        public SchedulePageViewModel()
        {
            GetTimes();
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

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRefreshing"));
            }
        }

        public Job SelectedJob { get; set; }

        #endregion

        #region Commands

        public ICommand GetTimesCommand => new Command(GetTimes);

        public ICommand RemoveSelectedCommand => new Command(() => {
            if (SelectedJob == null)
                Application.Current.MainPage.DisplayAlert("Error", "Please select a job to remove first.", "Dismiss");
            else
                RemoveItem(SelectedJob.Id);
        });

        public ICommand DeleteCommand => new Command<int>(RemoveItem);

        public ICommand AddNewCommand => new Command(async () =>
        {
            await Application.Current.MainPage.Navigation.PushAsync(new NewSchedulePage());
            GetTimes();
        });

        #endregion

        #region Methods

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
                if (!response.IsSuccessStatusCode)
                {
                    var res = await Application.Current.MainPage.DisplayAlert("Error Occured", 
                        "Unable to get times from the server. Makes sure your settings are correct.\nDo you want to proceed to settings page?", "Ok", "Dismiss");
                    if (res)
                    {
                        await Application.Current.MainPage.Navigation.PushAsync(new NavigationPage(new SettingsPage()));
                    }
                    return;
                }
                var job = await response.Content.ReadAsStringAsync();

                // Deserialize the jobs list and updat the Jobs collection
                Jobs = JsonConvert.DeserializeObject<ObservableCollection<Job>>(job);

                IsRefreshing = false;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error Occured", e.Message, "Dismiss");
            }
            finally { IsRefreshing = false; }
        }

        /// <summary>
        /// Removes the tapped item
        /// </summary>
        /// <param name="id">Id of the item to remove</param>
        private async void RemoveItem(int id)
        {
            var res = await Application.Current.MainPage.DisplayAlert("Warning!",
                "Are you sure you want to remove this job?", "Yes", "No");
            if (!res)
            {
                return;
            }

            try
            {
                var response = await NetworkHandler.GetResponseTask("remove?id=" + id, method: "DELETE");
                if (await response.Content.ReadAsStringAsync() == "OK")
                    GetTimes();
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error Occured", e.Message, "Dismiss");
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
    }
}