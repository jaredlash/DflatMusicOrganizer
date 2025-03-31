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
        bool EnableRunningJobs { get; set; }
        ICollection<Type> AcceptedRequestTypes { get; }

        event EventHandler<JobChangeEventArgs>? JobChanged;

        List<Task> RunJobs();

        bool TryCancelJob(int jobID);

        void CancelRunningJobs();
    }

    public interface IJobService<JobType> : IJobService where JobType : Job
    {
        void SubmitJobRequest(JobType job);
    }
}