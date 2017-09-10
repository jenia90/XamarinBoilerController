using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using BoilerController.Common.Helpers;
using BoilerController.Common.Models;
using Newtonsoft.Json;

namespace BoilerController.Common.Services
{
    public class BoilerServerService
    {
        private readonly int _devPin;

        public BoilerServerService() : this(17)
        {}

        public BoilerServerService(int devPin)
        {
            _devPin = devPin;
        }

        /// <summary>
        /// Gets the current state of the boiler
        /// </summary>
        /// <returns>Status object</returns>
        public async Task<Status> GetCurrentStateTask()
        {
            var response = await NetworkHandler.GetResponseTask("getstate?dev=" + _devPin);
            if(response == null || !response.IsSuccessStatusCode)
                throw new HttpRequestException("Unable to connect to server.");

            var content = await response.Content.ReadAsStringAsync();
            var status = JsonConvert.DeserializeObject<Status>(content);

            return status;
        }

        /// <summary>
        /// Sets the state desired state
        /// </summary>
        /// <param name="state">true for 'on'; false for 'off'</param>
        public async Task SetStateTask(bool state)
        {
            if (state)
            {
                await NetworkHandler.GetResponseTask("setstate?dev=" + _devPin + "&state=1");
            }
            else
            {
                await NetworkHandler.GetResponseTask("setstate?dev=" + _devPin + "&state=0");
            }
        }

        /// <summary>
        /// Gets the list of scheduled jobs
        /// </summary>
        /// <returns>Observable collection of Job objects</returns>
        public async Task<ObservableCollection<Job>> GetJobsTask()
        {
            var response = await NetworkHandler.GetResponseTask("gettimes");
            if (!response.IsSuccessStatusCode)
            {
                throw new UnauthorizedAccessException();
            }
            var job = await response.Content.ReadAsStringAsync();

            // Deserialize the jobs list and updat the Jobs collection
            return JsonConvert.DeserializeObject<ObservableCollection<Job>>(job);
        }

        /// <summary>
        /// Remove a job by a given ID number
        /// </summary>
        /// <param name="id">Job Id number</param>
        /// <returns>true if successful; false otherwise.</returns>
        public async Task<bool> RemoveJobTask(int id)
        {
            var response = await NetworkHandler.GetResponseTask("remove?id=" + id, method: "DELETE");
            if (await response.Content.ReadAsStringAsync() == "OK")
                return true;
            return false;
        }

        /// <summary>
        /// Creates a new job and sends request to add it to the server's schedule.
        /// </summary>
        /// <param name="start">Starting date and time string</param>
        /// <param name="end">Ending date and time string</param>
        /// <param name="type">Type of job (datetime\cron)</param>
        /// <param name="days">Days for cron job</param>
        public async Task SetScheduledJobTask(string start, string end, string type, IEnumerable<string> days)
        {
            var job = JsonConvert.SerializeObject(new Job
            {
                Pin = _devPin,
                Start = start,
                End = end,
                Type = type,
                DaysList = days
            });

            if (type == "datetime")
                type = "settime";
            else if (type == "cron")
                type = "addcron";

            // Send the request to the server and in case of success update the listview
            var response = await NetworkHandler.GetResponseTask(type, job, "POST");
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Remote operation failed.");
            }
        }
    }
}
