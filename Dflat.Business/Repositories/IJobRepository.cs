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
        void Add(Job job);

        
        JobType FindNextAvailable<JobType>() where JobType : Job;
        bool PrerequisitesFinished(int jobID);
    }
}
