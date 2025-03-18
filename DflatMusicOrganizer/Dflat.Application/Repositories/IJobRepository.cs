using Dflat.Application.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dflat.Application.Repositories
{
    public interface IJobRepository //: IRepository<Job>
    {
        void Add<JobType>(JobType job) where JobType : Job;

        void Update<JobType>(JobType job) where JobType : Job;

        bool RestartJob(int jobID);

        bool CancelJob(int jobID);

        /// <summary>
        /// Retrieves the next available job of the specified type.  Job is retrieved in the "Running" state.
        /// </summary>
        /// <typeparam name="JobType"></typeparam>
        /// <returns></returns>
        JobType GetNextAvailable<JobType>() where JobType : Job;

        //bool PrerequisitesFinished(int jobID);
        Job Get(int jobID);

        IEnumerable<JobInfo> GetJobInfoByCriteria(JobType jobType = JobType.None, JobStatus status = JobStatus.None);

        Task<IEnumerable<JobInfo>> GetJobInfoByCriteriaAsync(JobType jobType = JobType.None, JobStatus status = JobStatus.None);

        JobInfo GetJobInfo(int jobID);

        int GetJobCount();

        int GetJobCountByStatus(JobStatus jobStatus);

        int GetQueuedJobCount();

        int GetRunningJobCount();
    }
}
