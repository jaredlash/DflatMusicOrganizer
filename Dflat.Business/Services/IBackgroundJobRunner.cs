using System;
using Dflat.Business.Models;

namespace Dflat.Business.Services
{
    public interface IBackgroundJobRunner<JobType> where JobType : Job
    {
        Action<JobType> BackgroundWork { get; set; }
        Action<JobType> FinishWork { get; set; }

        void Run(JobType job);
    }
}