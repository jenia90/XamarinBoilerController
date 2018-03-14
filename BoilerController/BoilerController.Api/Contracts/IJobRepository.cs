using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoilerController.Api.Models;

namespace BoilerController.Api.Contracts
{
    public interface IJobRepository : IRepositoryBase<Job>
    {
        IEnumerable<Job> GetAllJobs();
        Job GetJobById(Guid jobId);
        void CreateJob(Job job);
        void UpdateJob(Job dbJob, Job job);
        void DeleteJob(Job job);
    }
}
