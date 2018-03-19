using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoilerController.Api.Models;

namespace BoilerController.Api.Extensions
{
    public static class JobExtensions
    {
        /// <summary>
        /// Returns true if object is null
        /// </summary>
        /// <param name="job">Job object</param>
        /// <returns>True if null; false otherwise</returns>
        public static bool IsObjectNull(this Job job)
        {
            return job == null;
        }

        /// <summary>
        /// Returns true if object is uninitialized
        /// </summary>
        /// <param name="job">Job object</param>
        /// <returns>true if uninitialized; false otherwise</returns>
        public static bool IsEmptyObject(this Job job)
        {
            return job.Id == Guid.Empty;
        }
    }
}
