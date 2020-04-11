using Dflat.Business.Factories;
using Dflat.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Business.Services
{
    public class FileMD5Service : JobService<FileMD5Job>, IJobService<FileMD5Job>
    {
        
        public FileMD5Service(IUnitOfWorkFactory unitOfWorkFactory,
                              IJobQueue jobQueue,
                              IBackgroundJobRunner<FileMD5Job> jobRunner)
            : base(unitOfWorkFactory, jobQueue, jobRunner)
        {
            MaxConcurrentJobs = 5;
        }


        public override void DoWork(FileMD5Job job)
        {
            throw new NotImplementedException();
        }

        public override void QueuePrerequisites(FileMD5Job job)
        {
            throw new NotImplementedException();
        }

        public override void SetupJob(FileMD5Job job)
        {
            throw new NotImplementedException();
        }
    }
}
