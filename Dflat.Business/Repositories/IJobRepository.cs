using Dflat.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Business.Repositories
{
    public interface IJobRepository : IRepository<Job>
    {
        /// <summary>
        /// Retrieves the next available job of the specified type.  Job is retrieved in the "Running" state.
        /// </summary>
        /// <typeparam name="JobType"></typeparam>
        /// <returns></returns>
        JobType GetNextAvailable<JobType>() where JobType : Job;

        bool PrerequisitesFinished(int jobID);
    }
}
