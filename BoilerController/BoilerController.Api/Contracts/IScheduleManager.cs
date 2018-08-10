using System;
using BoilerController.Api.Models;

namespace BoilerController.Api.Contracts
{
    public interface IScheduleManager
    {
        Guid AddJob(Device device, DateTime start, DateTime end, DayOfWeek[] days);
        void RemoveJob(Guid jobId);
    }
}
