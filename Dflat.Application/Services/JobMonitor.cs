using Dflat.Application.Models;
using Dflat.Application.Repositories;
using Dflat.Application.Services.JobServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Dflat.Application.Services
{
    public class JobMonitor : INotifyPropertyChanged
    {
        private readonly IJobRepository jobRepository;

        private readonly ICollection<IJobService> jobServices;

        public JobMonitor(IJobRepository jobRepository, IJobService<FileSourceFolderScanJob> folderScanService)
        {
            this.jobRepository = jobRepository;

            jobServices = new List<IJobService>
            {
                folderScanService
            };

            // Register listeners
            foreach (var jobService in jobServices)
            {
                jobService.JobChanged += ChildJobChanged;
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
                JobChanged?.Invoke(this, new JobChangeEventArgs { JobID = jobID, ChangeType = JobChangeEventArgs.JobChangeType.Updated });
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
                    JobChanged?.Invoke(this, new JobChangeEventArgs { JobID = jobID, ChangeType = JobChangeEventArgs.JobChangeType.Cancelled });
                    QueuedJobCount--;
                }
            }
        }

        #endregion

        #region Public events
        public event EventHandler<JobChangeEventArgs> JobChanged;
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

        #endregion

        #region Private methods

        private void ChildJobChanged(object sender, JobChangeEventArgs e)
        {
            switch (e.ChangeType)
            {
                case JobChangeEventArgs.JobChangeType.Submitted:
                    QueuedJobCount++;
                    break;

                case JobChangeEventArgs.JobChangeType.Started:
                    RunningJobCount++;
                    QueuedJobCount--;
                    break;

                case JobChangeEventArgs.JobChangeType.Finished:
                    RunningJobCount--;
                    FinishedJobCount++;
                    break;

                case JobChangeEventArgs.JobChangeType.Cancelled:
                    RunningJobCount--;
                    CancelledJobCount++;
                    break;
            }

            JobChanged?.Invoke(sender, e);
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void CallPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
