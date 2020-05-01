using Dflat.Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dflat.Application.Repositories
{
    public interface IJobRepository //: IRepository<Job>
    {
        void Add<JobType>(JobType job) where JobType : Job;

        void Update<JobType>(JobType job) where JobType : Job;

        /// <summary>
        /// Retrieves the next available job of the specified type.  Job is retrieved in the "Running" state.
        /// </summary>
        /// <typeparam name="JobType"></typeparam>
        /// <returns></returns>
        JobType GetNextAvailable<JobType>() where JobType : Job;

        //bool PrerequisitesFinished(int jobID);

        IEnumerable<JobInfo> GetJobInfoByCriteria(JobType jobType = JobType.None, JobStatus status = JobStatus.None);

        JobInfo GetJobInfo(int jobID);
    }
}
