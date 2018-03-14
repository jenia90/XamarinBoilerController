using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoilerController.Api.Models;

namespace BoilerController.Api.Extensions
{
    public static class JobExtensions
    {
        public static void Map(this Job dbJob, Job job)
        {
            dbJob.Start = job.Start;
            dbJob.DaysList = job.DaysList;
            dbJob.DeviceName = job.DeviceName;
            dbJob.End = job.End;
            dbJob.Pin = job.Pin;
            dbJob.Type = job.Type;
        }
    }
}
