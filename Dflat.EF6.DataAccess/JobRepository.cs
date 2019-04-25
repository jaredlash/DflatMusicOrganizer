using Dflat.Business.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dflat.Business.Models;

namespace Dflat.EF6.DataAccess
{
    public class JobRepository : IJobRepository
    {
        private readonly DataContext context;

        public JobRepository(DataContext context)
        {
            this.context = context;
        }
        
        public void Add(Job job)
        {
            context.Jobs.Add(job);
        }


        public Job Get(int id)
        {
            return context.Jobs.Find(id);
        }

        public List<Job> GetAll()
        {
            return context.Jobs.ToList();
        }

        public void Remove(int id)
        {
            throw new NotImplementedException();
        }

        public void Remove(Job item)
        {
            throw new NotImplementedException();
        }

        public List<JobType> GetCurrentlyRunning<JobType>() where JobType : Job
        {
            return context.Jobs.AsNoTracking().OfType<JobType>().Where((j) => j.Status == JobStatus.Running).ToList();
        }


        public List<JobType> GetReadyJobs<JobType>() where JobType : Job
        {
            return context.Jobs.AsNoTracking().OfType<JobType>().Where((j) => j.Status == JobStatus.Ready).ToList();
        }

        public JobType GetNextAvailable<JobType>() where JobType : Job
        {
            // First get the jobs that are currently running so that we can check that the job we pick which is queued is not equivalent
            // to one that is currently running.
            var runningJobs = GetCurrentlyRunning<JobType>();

            // Now get the jobs that are ready to be run
            var readyJobs = GetReadyJobs<JobType>();

            JobType nextJob = readyJobs.Where((j) => runningJobs.Any((r) => r.SameRequestAs(j)) == false).FirstOrDefault();

            if (nextJob == null)
                return null;


            // Update both a tracked and untracked version of the job so that we can save the new state.
            var tempJob = Get(nextJob.JobID).Status = nextJob.Status = JobStatus.Running;

            // Return the detached version
            return nextJob;
        }

        public bool PrerequisitesFinished(int jobID)
        {
            // Return True if we don't have any prerequisites
            if (!context.Jobs.Any((p) => p.DependentJobID == jobID))
                return true;

            // Make sure that all jobs that JobID is dependent on have finished
            return context.Jobs.Where((p) => p.DependentJobID == jobID).All((p) => p.Status == JobStatus.Success);
            
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
