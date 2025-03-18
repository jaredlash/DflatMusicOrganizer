using Dflat.Application.Models;
using Dflat.Application.Repositories;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace DflatCoreWPF.ViewModels
{
    public class JobDetailViewModel : ViewModelBase
    {
        private readonly IJobRepository jobRepository;
        
        
        private int jobID;
        private string jobType;
        private DateTime creationTime;
        private string description;
        private bool ignoreCache;
        private string status;
        private string output;
        private string errors;

        public JobDetailViewModel(IJobRepository jobRepository)
        {
            this.jobRepository = jobRepository;

            JobParameters = new BindingList<string>();
        }

        public ICommand InitializeCommand { get => new RelayCommand(() => LoadJobDetails()); }
        public ICommand CloseCommand { get => new RelayCommand(() => Close()); }

        #region Bindable properties

        public int JobID
        {
            get => jobID;
            set
            {
                jobID = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => JobIDDisplay);
                RaisePropertyChanged(() => Title);
            }
        }

        public string JobIDDisplay
        {
            get => $"(JobID {jobID})";
        }

        public string Title
        {
            get => $"Details for {JobType} {JobIDDisplay}";
        }

        public string JobType
        {
            get => jobType;
            private set
            {
                jobType = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => Title);
            }
        }

        public DateTime CreationTime
        {
            get => creationTime;
            private set
            {
                creationTime = value;
                RaisePropertyChanged();
            }
        }

        public string Description
        {
            get => description;
            private set
            {
                description = value;
                RaisePropertyChanged();
            }
        }

        public bool IgnoreCache
        {
            get => ignoreCache;
            private set
            {
                ignoreCache = value;
                RaisePropertyChanged();
            }
        }

        public string Status
        {
            get => status;
            private set
            {
                status = value;
                RaisePropertyChanged();
            }
        }

        public string Output
        {
            get => output;
            private set
            {
                output = value;
                RaisePropertyChanged();
            }
        }

        public string Errors
        {
            get => errors;
            private set
            {
                errors = value;
                RaisePropertyChanged();
            }
        }

        public BindingList<string> JobParameters { get; private set; }

        #endregion

        #region Public methods
        public void Clear()
        {
            Status = string.Empty;
            Description = string.Empty;
            Output = string.Empty;
            Errors = string.Empty;

            JobParameters.Clear();
        }

        #endregion

        #region Private methods

        private void LoadJobDetails()
        {
            var job = jobRepository.Get(JobID);

            if (job == null)
                return;

            this.JobType = job.ToString();
            CreationTime = job.CreationTime;
            Status = job.Status.ToString();
            Description = job.Description;
            IgnoreCache = job.IgnoreCache;
            Output = job.Output;
            Errors = job.Errors;

            switch (job)
            {
                case FileSourceFolderScanJob j:
                    JobParameters.Add($"FileSourceFolderID: {j.FileSourceFolderID}");
                    break;
            }
        }

        private void Close()
        {
            TryClose();
        }

        #endregion
    }
}
