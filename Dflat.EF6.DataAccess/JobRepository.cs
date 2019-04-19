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

        public Job Create()
        {
            throw new NotImplementedException();
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

        
        public JobType FindNextAvailable<JobType>() where JobType : Job
        {
            // Try to get a job with all of its prerequisites met (ready state)
            JobType job = context.Jobs.AsNoTracking().OfType<JobType>().Where((j) => j.Status == JobStatus.Ready).FirstOrDefault();


            return job;
        }

        public bool PrerequisitesFinished(int jobID)
        {
            // See if there are still jobs that this job is waiting on
            int count = context.Jobs.Where((p) => p.DependentJobID == jobID && p.Status != JobStatus.Success).Count();

            // If we have any, then the Prerequisites have not finished.
            if (count > 0)
                return false;
            else
                return true;
        }
    }
}
