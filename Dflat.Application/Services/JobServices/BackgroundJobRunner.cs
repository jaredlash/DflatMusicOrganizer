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

        public Task Run(JobType job)
        {
            var context = SynchronizationContext.Current;

            Task backgroundTask = new Task(() =>
            {
                BackgroundWork(job);
                context.Post((o) => FinishWork(job), null);
            }, TaskCreationOptions.LongRunning);

            backgroundTask.Start();

            return backgroundTask;
        }


        public Action<JobType> BackgroundWork { get; set; }

        public Action<JobType> FinishWork { get; set; }
    }
}
