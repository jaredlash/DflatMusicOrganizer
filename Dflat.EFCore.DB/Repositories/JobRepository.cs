using Dflat.Application.Models;
using Dflat.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dflat.EFCore.DB.Repositories
{
    public class JobRepository : IJobRepository
    {
        public void Add<JobType>(JobType job) where JobType : Job
        {
            throw new NotImplementedException();
        }

        public void Update<JobType>(JobType job) where JobType : Job 
        {
            throw new NotImplementedException();
            // This was in the old JobService, but should be here instead.
            // This update method is responsible for also updating the status of any parent
            // jobs for which this job is a prerequisite.

            // Only do this if the job status is Success

            //private void UpdateDependentJobStatus(JobType job)
            //{
            //    if (job.DependentJobID == null)
            //        return;

            //    int parentID = (int)job.DependentJobID;

            //    using (var unitOfWork = unitOfWorkFactory.Create())
            //    {
            //        // See if our parent job is ready to be run
            //        if (unitOfWork.JobRepository.PrerequisitesFinished(parentID))
            //        {
            //            var parentJob = unitOfWork.JobRepository.Get(parentID);
            //            parentJob.Status = JobStatus.Ready;
            //            unitOfWork.SaveChanges();
            //        }
            //    }
            //}
        }

        public JobType GetNextAvailable<JobType>() where JobType : Job
        {
            throw new NotImplementedException();
        }

        public bool PrerequisitesFinished(int jobID)
        {
            throw new NotImplementedException();
        }
    }
}
