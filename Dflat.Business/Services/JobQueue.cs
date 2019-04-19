using Dflat.Business.Factories;
using Dflat.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Business.Services
{
    public class JobQueue : IJobQueue
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public JobQueue(IUnitOfWorkFactory unitOfWorkFactory)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        public void Add(Job job)
        {
            using (var unitOfWork = unitOfWorkFactory.Create())
            {
                unitOfWork.JobRepository.Add(job);
                unitOfWork.SaveChanges();
            }
        }

        public JobType GetNextAvailableJob<JobType>() where JobType : Job
        {
            JobType job;

            using (var unitOfWork = unitOfWorkFactory.Create())
            {
                job = unitOfWork.JobRepository.FindNextAvailable<JobType>();
            }

            return job;
        }
    }
}
