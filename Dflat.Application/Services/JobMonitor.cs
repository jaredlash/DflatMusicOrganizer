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
            folderScanService.JobSubmitted += JobSubmitted;
            folderScanService.JobStarted += JobStarted;
            folderScanService.JobFinished += JobFinished;
        }

        private void JobSubmitted(object sender, EventArgs e)
        {
            QueuedJobCount++;
        }

        private void JobStarted(object sender, EventArgs e)
        {
            RunningJobCount++;
            QueuedJobCount--;
        }
        private void JobFinished(object sender, EventArgs e)
        {
            RunningJobCount--;
            FinishedJobCount++;
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
