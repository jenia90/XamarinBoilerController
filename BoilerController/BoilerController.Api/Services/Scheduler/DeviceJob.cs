using System;
using System.Threading.Tasks;
using BoilerController.Api.Models.Devices;
using Newtonsoft.Json;
using Quartz;

namespace BoilerController.Api.Services.Scheduler
{
    public class DeviceJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                // Get the job data
                var dataMap = context.JobDetail.JobDataMap;
                var deviceData = JsonConvert.DeserializeObject<OutputDevice>(dataMap.GetString("devicePin"));

                // Write the state to the device pin
                deviceData.State = dataMap.GetBooleanValue("state");
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }
    }
}
