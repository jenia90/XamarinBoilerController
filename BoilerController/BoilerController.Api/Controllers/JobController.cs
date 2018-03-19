using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoilerController.Api.Contracts;
using BoilerController.Api.Devices;
using BoilerController.Api.Extensions;
using BoilerController.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BoilerController.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Job")]
    public class JobController : Controller
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryWrapper _repoWrapper;
        private readonly IScheduleManager _scheduleManager;

        public JobController(ILoggerManager logger, IRepositoryWrapper repoWrapper, IScheduleManager scheduleManager)
        {
            _logger = logger;
            _repoWrapper = repoWrapper;
            _scheduleManager = scheduleManager;
        }

        [HttpGet]
        public IActionResult GetAllJobs()
        {
            try
            {
                var job = _repoWrapper.Job.GetAllJobs();
                _logger.LogInfo("Returned all jobs from database.");
                return Ok(job);
            }
            catch (Exception e)
            {
                _logger.LogError($"Something went wrong in GetAllJobs action: {e.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{id}", Name = "JobById")]
        public IActionResult GetJobById(Guid id)
        {
            try
            {
                var job = _repoWrapper.Job.GetJobById(id);
                if (job.IsEmptyObject())
                {
                    _logger.LogError($"Job with ID: {id} couldn't be found.");
                    return NotFound("Job with such ID couldn't be found.");
                }

                _logger.LogInfo($"Returned job with id: {id}");
                return Ok(job);
            }
            catch (Exception e)
            {
                _logger.LogError($"Something went wrong in GetJobById action: {e.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        public IActionResult CreateJob([FromBody] Job job)
        {
            try
            {
                if (job.IsObjectNull())
                {
                    _logger.LogError($"Job object sent from client is empty.");
                    return BadRequest($"Job object is null.");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"Job object sent from client is invalid.");
                    return BadRequest($"Job object is invalid.");
                }

                _repoWrapper.Job.CreateJob(job);
                //_scheduleManager.AddJob(new OutputDevice(), job.Start, job.End, job.DaysList.ToArray());
                return CreatedAtRoute("JobById", new { id = job.Id }, job);
            }
            catch (Exception e)
            {
                _logger.LogError($"Something went wrong inside CreateJob action: {e.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateJob(Guid id, [FromBody] Job job)
        {
            try
            {

                if (job.IsObjectNull())
                {
                    _logger.LogError($"Job object sent from client is empty.");
                    return BadRequest($"Job object is null.");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"Job object sent from client is invalid.");
                    return BadRequest($"Job object is invalid.");
                }

                var dbJob = _repoWrapper.Job.GetJobById(id);
                if (dbJob.IsEmptyObject())
                {
                    _logger.LogError($"Job with ID: {id} couldn't be found.");
                    return NotFound("Job with such ID couldn't be found.");
                }

                _repoWrapper.Job.UpdateJob(dbJob, job);
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError($"Something went wrong inside UpdateJob action: {e.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteJob(Guid id)
        {
            try
            {
                var job = _repoWrapper.Job.GetJobById(id);
                if (job.IsEmptyObject())
                {
                    _logger.LogError($"Job with ID: {id} couldn't be found.");
                    return NotFound("Job with such ID couldn't be found.");
                }

                _repoWrapper.Job.DeleteJob(job);
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError($"Something went wrong in DeleteJob action: {e.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
