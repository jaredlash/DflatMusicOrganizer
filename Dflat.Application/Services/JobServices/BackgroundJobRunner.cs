using Dflat.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dflat.Application.Services.JobServices
{
    public class BackgroundJobRunner<JobType> : IBackgroundJobRunner<JobType> where JobType : Job
    {

        public Task Run(JobType job, CancellationToken cancellationToken)
        {
            var context = SynchronizationContext.Current;

            Task backgroundTask = new Task(() =>
            {
                BackgroundWork(job, cancellationToken);
                context.Post((o) => FinishWork(job, cancellationToken), null);
            }, TaskCreationOptions.LongRunning);

            backgroundTask.Start();

            return backgroundTask;
        }


        public Action<JobType, CancellationToken> BackgroundWork { get; set; }

        public Action<JobType, CancellationToken> FinishWork { get; set; }
    }
}
