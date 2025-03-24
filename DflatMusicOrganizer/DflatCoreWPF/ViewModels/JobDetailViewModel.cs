using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dflat.Application.Models;
using Dflat.Application.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace DflatCoreWPF.ViewModels
{
    public partial class JobDetailViewModel : ViewModelBase
    {
        private readonly IJobRepository jobRepository;
        
        public JobDetailViewModel(IJobRepository jobRepository)
        {
            this.jobRepository = jobRepository;

            JobParameters = new BindingList<string>();
        }

        #region Bindable properties

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(JobIDDisplay))]
        [NotifyPropertyChangedFor(nameof(Title))]
        private int jobID;

        public string JobIDDisplay
        {
            get => $"(JobID {JobID})";
        }

        public string Title
        {
            get => $"Details for {JobType} {JobIDDisplay}";
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Title))]
        private string jobType;

        [ObservableProperty]
        private DateTime creationTime;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        private bool ignoreCache;

        [ObservableProperty]
        private string status;

        [ObservableProperty]
        private string output;

        [ObservableProperty]
        private string errors;

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

        [RelayCommand]
        private void Initialize()
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

        [RelayCommand]
        private void Close()
        {
            TryClose();
        }

        #endregion
    }
}
