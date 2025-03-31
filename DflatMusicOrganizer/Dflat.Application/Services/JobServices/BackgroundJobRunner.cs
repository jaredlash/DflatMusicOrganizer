using Dflat.Application.Models;
using System;
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
                try
                {
                    if (BackgroundWork is not null) BackgroundWork(job, cancellationToken);
                }
                catch (Exception ex)
                {
                    job.Status = JobStatus.Error;
                    job.Errors += $"Error running job: {ex.GetType()}: {ex.Message}";
                }

                if (FinishWork is null) return;

                if (context == null)
                {
                    // If we don't have a synchronization context, we can't post back to the UI thread
                    FinishWork(job, cancellationToken);
                    return;
                }

                // Post back to the UI thread to finish the job
                context.Post((o) => FinishWork(job, cancellationToken), null);
            });

            backgroundTask.Start();

            return backgroundTask;
        }


        public Action<JobType, CancellationToken>? BackgroundWork { get; set; }

        public Action<JobType, CancellationToken>? FinishWork { get; set; }
    }
}
