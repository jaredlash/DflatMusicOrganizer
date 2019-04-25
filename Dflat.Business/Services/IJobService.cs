using System;
using Dflat.Business.Models;

namespace Dflat.Business.Services
{
    public interface IJobService<JobType> where JobType : Job
    {
        int MaxConcurrentJobs { get; set; }
        int RunningJobCount { get; }

        event EventHandler JobFinished;

        void RunJobs();
        void SubmitJobRequest(JobType job);
    }
}