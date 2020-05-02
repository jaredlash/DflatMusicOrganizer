using Dflat.Application.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dflat.Application.Services.JobServices
{
    public interface IBackgroundJobRunner<JobType> where JobType : Job
    {
        Action<JobType> BackgroundWork { get; set; }
        Action<JobType> FinishWork { get; set; }

        Task Run(JobType job, CancellationToken cancellationToken);
    }
}