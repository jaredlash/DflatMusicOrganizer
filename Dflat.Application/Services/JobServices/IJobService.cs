using Dflat.Application.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dflat.Application.Services.JobServices
{
    public interface IJobService<JobType> where JobType : Job
    {
        int MaxConcurrentJobs { get; set; }
        int RunningJobCount { get; }

        event EventHandler<JobServiceEventArgs> JobSubmitted;
        event EventHandler<JobServiceEventArgs> JobStarted;
        event EventHandler<JobServiceEventArgs> JobFinished;

        List<Task> RunJobs();
        void SubmitJobRequest(JobType job);

        void TryCancelJob(int jobID);
    }
}