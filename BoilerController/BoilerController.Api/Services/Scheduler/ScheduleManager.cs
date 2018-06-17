using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using BoilerController.Api.Contracts;
using BoilerController.Api.Models.Devices;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;

namespace BoilerController.Api.Services.Scheduler
{
    /// <summary>
    /// Represents a Job Scheduling object which wraps the QuartzNET scheduler
    /// For more detail go to: http://www.quartz-scheduler.net/
    /// </summary>
    public class ScheduleManager : IScheduleManager
    {
        private readonly ILoggerManager _logger;
        private readonly IScheduler _scheduler;

        public ScheduleManager(ILoggerManager logger)
        {
            _logger = logger;

            // Initialize the scheduler
            var factory = new StdSchedulerFactory(new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            });
            _scheduler = factory.GetScheduler().Result;
            _scheduler.Start();
        }

        /// <summary>
        /// Schedules activation-deactivation for a specified device
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="device"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public Guid AddJob(Device device, DateTime start, DateTime end, DayOfWeek[] days)
        {
            try
            {
                var id = Guid.NewGuid();
                var deviceData = JsonConvert.SerializeObject(device);

                KeyValuePair<IJobDetail, ITrigger> activateJobKey, deactivateJobKey;

                // if days of week specified then set a cron job; otherwise set one time job
                if (days.Length > 0)
                {
                    activateJobKey = CreateCronJob(id, deviceData, start, days, true);
                    deactivateJobKey = CreateCronJob(id, deviceData, end, days, false);
                }
                else
                {
                    activateJobKey = CreateOneTimeJob(id, deviceData, start, true);
                    deactivateJobKey = CreateOneTimeJob(id, deviceData, end, false);
                }

                _scheduler.ScheduleJob(activateJobKey.Key, activateJobKey.Value);
                _scheduler.ScheduleJob(deactivateJobKey.Key, deactivateJobKey.Value);

                _logger.LogInfo($"New job with ID {id} added");

                return id;
            }
            catch (Exception e)
            {
                _logger.LogError($"Scheduler error: {e.Message}");
                return Guid.Empty;
            }
        }

        public async void RemoveJob(Guid jobId)
        {
            var result = _scheduler.DeleteJobs(new List<JobKey>
            {
                new JobKey("Activate", jobId.ToString()),
                new JobKey("Deactivate", jobId.ToString())
            });

            if (await result)
            {
                _logger.LogInfo($"Jobs with in group: {jobId}, were deleted successfully");
            }
            else
            {
                _logger.LogError($"Couldn't find or delete jobs in group: {jobId}");
            }
        }

        private KeyValuePair<IJobDetail, ITrigger> CreateCronJob(Guid id, 
            string deviceData, 
            DateTime time,
            DayOfWeek[] days, 
            bool isActivate)
        {
            var type = isActivate ? "Activate" : "Deactivate";
            var job = JobBuilder.Create<DeviceJob>()
                .UsingJobData("state", isActivate)
                .UsingJobData("deviceData", deviceData)
                .WithIdentity(type, id.ToString())
                .Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity(type, id.ToString())
                .WithSchedule(CronScheduleBuilder
                    .AtHourAndMinuteOnGivenDaysOfWeek(time.Hour, time.Minute, days))
                .Build();

            return new KeyValuePair<IJobDetail, ITrigger>(job, trigger);
        }

        private KeyValuePair<IJobDetail, ITrigger> CreateOneTimeJob(Guid id,
            string deviceData,
            DateTime time,
            bool isActivate)
        {
            var type = isActivate ? "Activate" : "Deactivate";
            var job = JobBuilder.Create<DeviceJob>()
                .UsingJobData("state", isActivate)
                .UsingJobData("deviceData", deviceData)
                .WithIdentity(type, id.ToString())
                .Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity("Activate", id.ToString())
                .StartAt(time)
                .Build();

            return new KeyValuePair<IJobDetail, ITrigger>(job, trigger);
        }
    }
}