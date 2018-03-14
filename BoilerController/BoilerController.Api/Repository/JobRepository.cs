using System;
using System.Collections.Generic;
using System.Linq;
using BoilerController.Api.Contracts;
using BoilerController.Api.Extensions;
using BoilerController.Api.Models;

namespace BoilerController.Api.Repository
{
    public class JobRepository : RepositoryBase<Job>, IJobRepository
    {
        public JobRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Job> GetAllJobs()
        {
            return FindAll().OrderBy(j => DateTime.Parse(j.Start));
        }

        public Job GetJobById(Guid jobId)
        {
            return FindByCondition(j => j.Id.Equals(jobId))
                .DefaultIfEmpty(new Job())
                .FirstOrDefault();
        }

        public void CreateJob(Job job)
        {
            job.Id = Guid.NewGuid();
            Create(job);
            Save();
        }

        public void UpdateJob(Job dbJob, Job job)
        {
            dbJob.Map(job);
            Update(job);
            Save();
        }

        public void DeleteJob(Job job)
        {
            Delete(job);
            Save();
        }
    }
}
