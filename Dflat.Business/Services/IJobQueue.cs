using Dflat.Business.Models;

namespace Dflat.Business.Services
{

    /// <summary>
    /// A Job Queue manages storing and retrieving jobs.
    /// </summary>
    /// <typeparam name="JobType">Type of Job this queue holds</typeparam>
    public interface IJobQueue<JobType> where JobType : Job
    {
        /// <summary>
        /// Adds a job to the queue.
        /// 
        /// This may be run in a separate thread than in which the queue was created.
        /// </summary>
        /// <param name="job">Job to add to the queue.</param>
        void Add(JobType job);

        /// <summary>
        /// Retrieves the next available job.  This job should be independent of any tracking/storage implementation (e.g.,
        /// the Entity Framework implementation should Detach the Job from the DbContext).
        /// </summary>
        /// <returns>Job request.  This job should be independent of any tracking/storage implementation.</returns>
        JobType GetNextAvailableJob();
    }
}