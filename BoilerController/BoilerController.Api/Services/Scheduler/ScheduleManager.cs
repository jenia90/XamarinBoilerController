using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using BoilerController.Api.Contracts;
using BoilerController.Api.Devices;
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
        public Guid AddJob(IDevice device, DateTime start, DateTime end, DayOfWeek[] days)
        {
            try
            {
                var id = Guid.NewGuid();
                var deviceData = JsonConvert.SerializeObject(device);
                ITrigger activationTrigger, deactivationTrigger;

                // Create activation and deactivation jobs
                var activationJob = JobBuilder.Create<DeviceJob>()
                    .UsingJobData("state", true)
                    .UsingJobData("deviceData", deviceData)
                    .WithIdentity("Activate", id.ToString())
                    .Build();
                var deactivationJob = JobBuilder.Create<DeviceJob>()
                    .UsingJobData("state", false)
                    .UsingJobData("deviceData", deviceData)
                    .WithIdentity("Deactivate", id.ToString())
                    .Build();

                // if days of week specified then set a cron job; otherwise set one time job
                if (days.Length > 0)
                {
                    activationTrigger = TriggerBuilder.Create()
                        .WithIdentity("Activate", id.ToString())
                        .WithSchedule(CronScheduleBuilder
                            .AtHourAndMinuteOnGivenDaysOfWeek(start.Hour, start.Minute, days))
                        .Build();
                    deactivationTrigger = TriggerBuilder.Create()
                        .WithIdentity("Deactivate", id.ToString())
                        .WithSchedule(CronScheduleBuilder
                            .AtHourAndMinuteOnGivenDaysOfWeek(start.Hour, start.Minute, days))
                        .Build();
                }
                else
                {
                    activationTrigger = TriggerBuilder.Create()
                        .WithIdentity("Activate", id.ToString())
                        .StartAt(start)
                        .Build();
                    deactivationTrigger = TriggerBuilder.Create()
                        .WithIdentity("Deactivate", id.ToString())
                        .StartAt(end)
                        .Build();
                }

                _scheduler.ScheduleJob(activationJob, activationTrigger);
                _scheduler.ScheduleJob(deactivationJob, deactivationTrigger);

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
    }
}
