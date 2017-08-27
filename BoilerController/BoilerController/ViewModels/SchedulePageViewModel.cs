using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
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

        #endregion

        #region Commands

        public ICommand GetTimesCommand => new Command(GetTimes);
        public ICommand DeleteCommand => new Command<int>(RemoveItem);
        public ICommand AddNewCommand => new Command(ShowAddEvent);

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

        private async void ShowAddEvent()
        {
            await App.Current.MainPage.Navigation.PushAsync(new NewSchedulePage());
            GetTimes();
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
    }
}