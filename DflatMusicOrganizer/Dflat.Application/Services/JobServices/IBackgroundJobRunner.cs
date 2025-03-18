using Dflat.Application.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dflat.Application.Services.JobServices
{
    public interface IBackgroundJobRunner<JobType> where JobType : Job
    {
        Action<JobType, CancellationToken> BackgroundWork { get; set; }
        Action<JobType, CancellationToken> FinishWork { get; set; }

        Task Run(JobType job, CancellationToken cancellationToken);
    }
}