﻿using Dflat.Application.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dflat.Application.Services.JobServices
{
    public interface IJobService<JobType> where JobType : Job
    {
        int MaxConcurrentJobs { get; set; }
        int RunningJobCount { get; }

        event EventHandler JobSubmitted;
        event EventHandler JobStarted;
        event EventHandler JobFinished;

        List<Task> RunJobs();
        void SubmitJobRequest(JobType job);
    }
}