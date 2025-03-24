using Dflat.Application.Models;
using Dflat.Application.Repositories;
using Dflat.Application.Services.JobServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Dflat.Application.Services;

public class JobMonitor : INotifyPropertyChanged
{
    private readonly IJobRepository jobRepository;
    private readonly ICollection<IJobService> jobServices;

    public JobMonitor(IJobRepository jobRepository, IJobService<FileSourceFolderScanJob> folderScanService, IJobService<MD5Job> md5Service)
    {
        this.jobRepository = jobRepository;

        jobServices =
        [
            folderScanService,
            md5Service
        ];

        // Register listeners
        foreach (var jobService in jobServices)
        {
            jobService.JobChanged += HandleJobChanged;
        }
    }

    #region Public methods
    public void RestartJob(int jobID)
    {
        var success = jobRepository.RestartJob(jobID);
        
        // Notify appropriate listeners that we changed the job
        // Tell the appropriate JobService to run ready jobs now that we've made the job ready
        if (success)
        {
            HandleJobChanged(this, new JobChangeEventArgs { JobID = jobID, ChangeType = JobChangeEventArgs.JobChangeType.Updated });
            var job = jobRepository.Get(jobID);

            foreach (var jobService in jobServices)
            {
                if (jobService.AcceptedRequestTypes.Contains(job.GetType()))
                {
                    jobService.RunJobs();
                    break;
                }
            }
        }
    }

    public void CancelJob(int jobID)
    {
        if (jobServices.Any((s) => s.TryCancelJob(jobID)) == false)
        {
            // None of the job services were currently running the specified job
            // Cancel the job in the repository
            var success = jobRepository.CancelJob(jobID);
            if (success)
            {
                HandleJobChanged(this, new JobChangeEventArgs { JobID = jobID, ChangeType = JobChangeEventArgs.JobChangeType.Cancelled });
            }
        }
    }

    /// <summary>
    /// Stops all job processing in the Job Services.
    /// 
    /// No queued jobs will be run.
    /// </summary>
    /// <param name="cancelRunningJobs">True if running jobs should be cancelled and not finished.</param>
    public void StopProcessing(bool cancelRunningJobs = false)
    {
        ProcessingIsEnabled = false;
        foreach (var jobService in jobServices)
        {
            jobService.EnableRunningJobs = false;
            if (cancelRunningJobs)
                jobService.CancelRunningJobs();
        }
    }

    public void StartProcessing()
    {
        ProcessingIsEnabled = true;
        QueuedJobCount = jobRepository.GetQueuedJobCount();
        RunningJobCount = jobRepository.GetRunningJobCount();
        foreach (var jobService in jobServices)
        {
            jobService.EnableRunningJobs = true;
            jobService.RunJobs();
        }
    }
    #endregion

    #region Public events
    public event EventHandler<JobChangeEventArgs>? JobChanged;
    #endregion

    #region Bindable properties
    private int runningJobCount;
    public int RunningJobCount
    {
        get { return runningJobCount; }
        set
        {
            runningJobCount = value;
            CallPropertyChanged(nameof(RunningJobCount));
        }
    }

    private int queuedJobCount;
    public int QueuedJobCount
    {
        get { return queuedJobCount; }
        set
        {
            queuedJobCount = value;
            CallPropertyChanged(nameof(QueuedJobCount));
        }
    }

    private int finishedJobCount;
    public int FinishedJobCount
    {
        get { return finishedJobCount; }
        set
        {
            finishedJobCount = value;
            CallPropertyChanged(nameof(FinishedJobCount));
        }
    }

    private int cancelledJobCount;
    public int CancelledJobCount
    {
        get { return cancelledJobCount; }
        set
        {
            cancelledJobCount = value;
            CallPropertyChanged(nameof(CancelledJobCount));
        }
    }

    public bool ProcessingIsEnabled { get; private set; }


    #endregion

    #region Private methods

    private void HandleJobChanged(object? sender, JobChangeEventArgs e)
    {
        switch (e.ChangeType)
        {
            case JobChangeEventArgs.JobChangeType.Submitted:
            case JobChangeEventArgs.JobChangeType.Updated:
                QueuedJobCount = jobRepository.GetQueuedJobCount();
                break;

            case JobChangeEventArgs.JobChangeType.Started:
                RunningJobCount = GetRunningJobCountFromServices();
                QueuedJobCount = jobRepository.GetQueuedJobCount();
                break;

            case JobChangeEventArgs.JobChangeType.Finished:
                RunningJobCount = GetRunningJobCountFromServices();
                FinishedJobCount++;
                break;

            case JobChangeEventArgs.JobChangeType.Cancelled:
                RunningJobCount = GetRunningJobCountFromServices();
                CancelledJobCount++;
                break;
        }

        JobChanged?.Invoke(sender, e);
    }


    private int GetRunningJobCountFromServices()
    {
        return jobServices.Sum((i) => i.RunningJobCount);
    }

    #endregion

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;
    private void CallPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    #endregion
}
