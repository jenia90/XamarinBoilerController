using BoilerController.Api.Models;

namespace BoilerController.Api.Extensions
{
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Maps properties of a given job to job in database.
        /// </summary>
        /// <param name="dbJob">DB job object</param>
        /// <param name="job">New job object</param>
        public static void Map(this Job dbJob, Job job)
        {
            dbJob.Start = job.Start;
            dbJob.DaysList = job.DaysList;
            dbJob.DeviceName = job.DeviceName;
            dbJob.End = job.End;
            dbJob.Type = job.Type;
        }

        /// <summary>
        /// Maps properties of a given device to device in database.
        /// </summary>
        /// <param name="dbDevice">DB device object</param>
        /// <param name="device">New device object</param>
        public static void Map(this Device dbDevice, Device device)
        {
            dbDevice.DevicePin = device.DevicePin;
            dbDevice.DeviceName = device.DeviceName;
        }
    }
}
