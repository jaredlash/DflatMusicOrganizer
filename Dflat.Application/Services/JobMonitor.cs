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
                jobService.JobSubmitted += ChildJobSubmitted;
                jobService.JobStarted += ChildJobStarted;
                jobService.JobFinished += ChildJobFinished;
            }
        }

        #region Public methods
        public void CancelJob(int jobID)
        {
            if (jobServices.Any((s) => s.TryCancelJob(jobID)) == false)
            {
                // None of the job services were currently running the specified job
                // Cancel the job in the repository
                //jobRepository.CancelJob(jobID);
            }
        }

        #endregion

        #region Public events
        public event EventHandler<JobChangeEventArgs> JobSubmitted;
        public event EventHandler<JobChangeEventArgs> JobStarted;
        public event EventHandler<JobChangeEventArgs> JobFinished;
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
        #endregion

        #region Private methods

        private void ChildJobSubmitted(object sender, JobChangeEventArgs e)
        {
            QueuedJobCount++;
            JobSubmitted?.Invoke(this, e);
        }

        private void ChildJobStarted(object sender, JobChangeEventArgs e)
        {
            RunningJobCount++;
            QueuedJobCount--;
            JobStarted?.Invoke(this, e);
        }
        private void ChildJobFinished(object sender, JobChangeEventArgs e)
        {
            RunningJobCount--;
            FinishedJobCount++;
            JobFinished?.Invoke(this, e);
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
