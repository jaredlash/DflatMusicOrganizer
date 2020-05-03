using Dflat.Application.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dflat.Application.Services.JobServices
{
    public interface IJobService
    {
        int MaxConcurrentJobs { get; set; }
        int RunningJobCount { get; }


        event EventHandler<JobChangeEventArgs> JobSubmitted;
        event EventHandler<JobChangeEventArgs> JobStarted;
        event EventHandler<JobChangeEventArgs> JobFinished;

        List<Task> RunJobs();

        bool TryCancelJob(int jobID);
    }

    public interface IJobService<JobType> : IJobService where JobType : Job
    {
        void SubmitJobRequest(JobType job);
    }
}