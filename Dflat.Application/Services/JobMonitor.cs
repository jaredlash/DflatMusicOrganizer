using Dflat.Application.Models;
using Dflat.Application.Repositories;
using Dflat.Application.Services.JobServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Dflat.Application.Services
{
    public class JobMonitor : INotifyPropertyChanged
    {
        private readonly IJobRepository jobRepository;

        public JobMonitor(IJobRepository jobRepository, IJobService<FileSourceFolderScanJob> folderScanService)
        {
            this.jobRepository = jobRepository;


            // Register listeners
            folderScanService.JobSubmitted += ChildJobSubmitted;
            folderScanService.JobStarted += ChildJobStarted;
            folderScanService.JobFinished += ChildJobFinished;
        }


        public event EventHandler<JobServiceEventArgs> JobSubmitted;
        public event EventHandler<JobServiceEventArgs> JobStarted;
        public event EventHandler<JobServiceEventArgs> JobFinished;



        private void ChildJobSubmitted(object sender, JobServiceEventArgs e)
        {
            QueuedJobCount++;
            JobSubmitted?.Invoke(this, e);
        }

        private void ChildJobStarted(object sender, JobServiceEventArgs e)
        {
            RunningJobCount++;
            QueuedJobCount--;
            JobStarted?.Invoke(this, e);
        }
        private void ChildJobFinished(object sender, JobServiceEventArgs e)
        {
            RunningJobCount--;
            FinishedJobCount++;
            JobFinished?.Invoke(this, e);
        }


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




        public event PropertyChangedEventHandler PropertyChanged;

        private void CallPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
