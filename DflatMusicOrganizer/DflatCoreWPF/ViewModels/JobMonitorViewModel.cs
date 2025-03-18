using Dflat.Application.Models;
using Dflat.Application.Repositories;
using Dflat.Application.Services;
using Dflat.Application.Services.JobServices;
using DflatCoreWPF.Views;
using DflatCoreWPF.WindowService;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace DflatCoreWPF.ViewModels
{
    public class JobMonitorViewModel : ViewModelBase
    {
        private readonly JobMonitor jobMonitor;
        private readonly IJobRepository jobRepository;
        private readonly IWindowService windowService;
        private readonly JobDetailViewModel jobDetailViewModel;
        private readonly AlertDialogViewModel alertDialogViewModel;

        private BindingList<JobInfo> jobInfoList;
        private JobInfo selectedJobInfo;
        private JobType displayedJobType;
        private JobStatus displayedJobStatus;


        public JobMonitorViewModel(JobMonitor jobMonitor,
                                   IJobRepository jobRepository,
                                   IWindowService windowService,
                                   JobDetailViewModel jobDetailViewModel,
                                   AlertDialogViewModel alertDialogViewModel)
        {
            this.jobMonitor = jobMonitor;
            this.jobRepository = jobRepository;
            this.windowService = windowService;
            this.jobDetailViewModel = jobDetailViewModel;
            this.alertDialogViewModel = alertDialogViewModel;
            JobInfoList = new BindingList<JobInfo>();

            jobMonitor.JobChanged += JobMonitor_JobStatusChange;
        }


        private async Task Initialize()
        {
            displayedJobType = JobType.None;
            displayedJobStatus = JobStatus.None;

            await LoadJobs();
        }

        public override void OnClose()
        {
            JobInfoList.Clear();
            SelectedJobInfo = null;
        }


        #region Commands

        public ICommand InitializeCommand { get => new RelayCommand(async () => await Initialize()); }

        public ICommand ViewJobDetailsCommand { get => new RelayCommand(() => ViewJobDetails()); }

        public ICommand DisplayJobTypeCommand { get => new RelayCommand<JobType>(async (jobType) => await DisplayJobType(jobType)); }

        public ICommand DisplayJobStatusCommand { get => new RelayCommand<JobStatus>(async (jobStatus) => await DisplayJobStatus(jobStatus)); }

        public ICommand RestartSelectedJobsCommand { get => new RelayCommand(() => RestartSelectedJobs()); }
        public ICommand CancelSelectedJobsCommand { get => new RelayCommand(() => CancelSelectedJobs()); }
        public ICommand RemoveSelectedJobsCommand { get => new RelayCommand(() => RemoveSelectedJobs()); }

        public ICommand ChangeProcessingStatusCommand { get => new RelayCommand<bool>((enable) => ChangeProcessingStatus(enable)); }
        #endregion


        #region Bindable Properties


        public BindingList<JobInfo> JobInfoList
        {
            get => jobInfoList;
            private set
            {
                jobInfoList = value;
                RaisePropertyChanged();
            }
        }


        public JobInfo SelectedJobInfo
        {
            get { return selectedJobInfo; }
            set
            {
                selectedJobInfo = value;
                RaisePropertyChanged(() => SelectedJobInfo);
                RaisePropertyChanged(() => CanRestartSelectedJobs);
                RaisePropertyChanged(() => CanCancelSelectedJobs);
            }
        }

        public bool CanRestartSelectedJobs
        {
            get => SelectedJobInfo?.Status != JobStatus.Running &&
                SelectedJobInfo?.Status != JobStatus.Queued &&
                SelectedJobInfo?.Status != JobStatus.Ready;
        }

        public bool CanCancelSelectedJobs
        {
            get => SelectedJobInfo?.Status != JobStatus.Success &&
                SelectedJobInfo?.Status != JobStatus.SuccessWithErrors &&
                SelectedJobInfo?.Status != JobStatus.Cancelled;
        }


        public bool DisplayAllTypes { get =>  displayedJobType == JobType.None; }
        public bool DisplayFileSourceFolderScanJob { get => displayedJobType == JobType.FileSourceFolderScanJob; }
        public bool DisplayMD5Job { get => displayedJobType == JobType.MD5Job; }


        public bool DisplayAllStatuses { get => displayedJobStatus == JobStatus.None; }
        public bool DisplayQueuedJobs { get => displayedJobStatus == JobStatus.Queued; }
        public bool DisplayReadyJobs { get => displayedJobStatus == JobStatus.Ready; }
        public bool DisplayRunningJobs { get => displayedJobStatus == JobStatus.Running; }
        public bool DisplaySuccessfulJobs { get => displayedJobStatus == JobStatus.Success; }
        public bool DisplaySuccessWithErrorJobs { get => displayedJobStatus == JobStatus.SuccessWithErrors; }
        public bool DisplayErroredJobs { get => displayedJobStatus == JobStatus.Error; }
        public bool DisplayCancelledJobs { get => displayedJobStatus == JobStatus.Cancelled; }
        
        public bool ProcessingIsEnabled { get => jobMonitor.ProcessingIsEnabled; }
        public bool ProcessingIsStopped { get => jobMonitor.ProcessingIsEnabled == false; }
        #endregion

        #region Private methods
        private async Task LoadJobs()
        {
            JobInfoList.Clear();

            try
            {
                IEnumerable<JobInfo> jobList = await jobRepository.GetJobInfoByCriteriaAsync(displayedJobType, displayedJobStatus);

                foreach (JobInfo jobInfo in jobList)
                    JobInfoList.Add(jobInfo);
            }
            catch (Exception ex)
            {
                alertDialogViewModel.Title = "Problem loading jobs"; ;
                alertDialogViewModel.Message = $"A problem was encountered when loading the jobs: {ex.Message}";
                windowService.ShowDialog(alertDialogViewModel);
            }
            RaisePropertyChanged(() => CanRestartSelectedJobs);
            RaisePropertyChanged(() => CanCancelSelectedJobs);
        }

        private void ViewJobDetails()
        {
            jobDetailViewModel.JobID = SelectedJobInfo.JobID;
            windowService.ShowDialog(jobDetailViewModel);
            jobDetailViewModel.Clear();
        }

 


        private async Task DisplayJobType(JobType jobType)
        {
            displayedJobType = jobType;
            await LoadJobs();

            RaisePropertyChanged(() => DisplayAllTypes);
            RaisePropertyChanged(() => DisplayFileSourceFolderScanJob);
            RaisePropertyChanged(() => DisplayMD5Job);
        }

        private async Task DisplayJobStatus(JobStatus status)
        {
            displayedJobStatus = status;
            await LoadJobs();

            RaisePropertyChanged(() => DisplayAllStatuses);
            RaisePropertyChanged(() => DisplayQueuedJobs);
            RaisePropertyChanged(() => DisplayReadyJobs);
            RaisePropertyChanged(() => DisplayRunningJobs);
            RaisePropertyChanged(() => DisplaySuccessfulJobs);
            RaisePropertyChanged(() => DisplaySuccessWithErrorJobs);
            RaisePropertyChanged(() => DisplayErroredJobs);
            RaisePropertyChanged(() => DisplayCancelledJobs);
        }

        private void RestartSelectedJobs()
        {
            if (SelectedJobInfo != null)
            {
                jobMonitor.RestartJob(SelectedJobInfo.JobID);
            }
        }

        private void CancelSelectedJobs()
        {
            if (SelectedJobInfo != null)
            {
                jobMonitor.CancelJob(SelectedJobInfo.JobID);
            }
        }

        private void RemoveSelectedJobs()
        {
            alertDialogViewModel.Title = "Remove Selected Jobs"; ;
            alertDialogViewModel.Message = $"Attempt to remove: {SelectedJobInfo?.JobID}";
            windowService.ShowDialog(alertDialogViewModel);
        }

        private void ChangeProcessingStatus(bool enable)
        {
            if (enable)
                jobMonitor.StartProcessing();
            else
                jobMonitor.StopProcessing();
            RaisePropertyChanged(() => ProcessingIsEnabled);
            RaisePropertyChanged(() => ProcessingIsStopped);
        }

        private void JobMonitor_JobStatusChange(object sender, JobChangeEventArgs e)
        {
            JobInfo jobInfo = jobRepository.GetJobInfo(e.JobID);

            // JobInfo to update
            var foundJobInfo = JobInfoList.FirstOrDefault((j) => j.JobID == jobInfo.JobID);

            if ((displayedJobStatus == JobStatus.None || jobInfo.Status == displayedJobStatus) &&
                (displayedJobType == JobType.None || jobInfo.JobType == displayedJobType))
            {
                // This JobInfo fits the display criteria for the current list
                if (foundJobInfo != null)
                {
                    foundJobInfo.SetFromExisting(jobInfo);
                }
                else // It should be in the list but isn't.
                    JobInfoList.Insert(0, jobInfo);
            }
            else
            {
                // It does not fit the display criteria for the current list, so remove it if found
                if (foundJobInfo != null) JobInfoList.Remove(foundJobInfo);
            }

            RaisePropertyChanged(() => CanRestartSelectedJobs);
            RaisePropertyChanged(() => CanCancelSelectedJobs);
        }

        #endregion
    }
}
