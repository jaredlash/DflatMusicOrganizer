using Dflat.Business.Factories;
using Dflat.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Business.Services
{
    public class FileChromaprintService : JobService<FileChromaprintJob>, IJobService<FileChromaprintJob>
    {
        public FileChromaprintService(IUnitOfWorkFactory unitOfWorkFactory,
                                      IJobQueue jobQueue,
                                      IBackgroundJobRunner<FileChromaprintJob> jobRunner)
            : base(unitOfWorkFactory, jobQueue, jobRunner)
        {
            MaxConcurrentJobs = 5;
        }

        public override void DoWork(FileChromaprintJob job)
        {
            if (job is null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            throw new NotImplementedException();
        }

        public override void QueuePrerequisites(FileChromaprintJob job)
        {
            // This job type ha no prerequisites
        }

        public override void SetupJob(FileChromaprintJob job)
        {
            if (job is null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            throw new NotImplementedException();
        }
    }
}
