using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

namespace Dflat.Application.Models
{
    /// <summary>
    /// Contains Job information without the larger fields
    /// 
    /// Omits the "errors" and "output" fields.
    /// </summary>
    public class JobInfo : INotifyPropertyChanged
    {
        private int jobID;
        private JobType jobType;
        private DateTime creationTime;
        private string description;
        private bool ignoreCache;
        private JobStatus status;

        public int JobID
        {
            get => jobID;
            set
            {
                jobID = value;
                CallPropertyChanged(nameof(JobID));
            }
        }
        public JobType JobType
        {
            get => jobType;
            set
            {
                jobType = value;
                CallPropertyChanged(nameof(JobType));
            }
        }
        public DateTime CreationTime
        {
            get => creationTime;
            set
            {
                creationTime = value;
                CallPropertyChanged(nameof(CreationTime));
            }
        }
        public string Description
        {
            get => description;
            set
            {
                description = value;
                CallPropertyChanged(nameof(Description));
            }
        }
        public bool IgnoreCache
        {
            get => ignoreCache;
            set
            {
                ignoreCache = value;
                CallPropertyChanged(nameof(IgnoreCache));
            }
        }
        public JobStatus Status
        {
            get => status;
            set
            {
                status = value;
                CallPropertyChanged(nameof(Status));
            }
        }


        public void SetFromExisting(JobInfo jobInfo)
        {
            JobID = jobInfo.JobID;
            JobType = jobInfo.JobType;
            CreationTime = jobInfo.CreationTime;
            Description = jobInfo.Description;
            IgnoreCache = jobInfo.IgnoreCache;
            Status = jobInfo.Status;
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void CallPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
