using System;
using BoilerController.Api.Devices;

namespace BoilerController.Api.Contracts
{
    public interface IScheduleManager
    {
        Guid AddJob(IDevice device, DateTime start, DateTime end, DayOfWeek[] days);
        void RemoveJob(Guid jobId);
    }
}
