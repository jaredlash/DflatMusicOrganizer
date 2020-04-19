using Dflat.Application.Models;

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
    }
}
