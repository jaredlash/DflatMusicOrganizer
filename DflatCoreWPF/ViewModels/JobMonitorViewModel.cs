﻿using Dflat.Application.Models;
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

            jobMonitor.JobSubmitted += JobMonitor_JobSubmitted;
            jobMonitor.JobStarted += JobMonitor_JobStatusChange;
            jobMonitor.JobFinished += JobMonitor_JobStatusChange;
        }


        private void Initialize()
        {
            displayedJobType = JobType.None;
            displayedJobStatus = JobStatus.None;

            LoadJobs();
        }


        #region Commands

        public ICommand InitializeCommand { get => new RelayCommand(() => Initialize()); }

        public ICommand ViewJobDetailsCommand { get => new RelayCommand(() => ViewJobDetails()); }

        public ICommand DisplayJobTypeCommand { get => new RelayCommand<JobType>((jobType) => DisplayJobType(jobType)); }

        public ICommand DisplayJobStatusCommand { get => new RelayCommand<JobStatus>((jobStatus) => DisplayJobStatus(jobStatus)); }

        public ICommand RestartSelectedJobsCommand { get => new RelayCommand(() => RestartSelectedJobs()); }
        public ICommand CancelSelectedJobsCommand { get => new RelayCommand(() => CancelSelectedJobs()); }
        public ICommand RemoveSelectedJobsCommand { get => new RelayCommand(() => RemoveSelectedJobs()); }
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
                RaisePropertyChanged(() => CanCancelSelectedJobs);
            }
        }

        public bool CanCancelSelectedJobs
        {
            get => SelectedJobInfo?.Status != JobStatus.Success && SelectedJobInfo?.Status != JobStatus.SuccessWithErrors;
        }


        public bool DisplayAllTypes { get =>  displayedJobType == JobType.None; }
        public bool DisplayFileSourceFolderScanJob { get => displayedJobType == JobType.FileSourceFolderScanJob; }


        public bool DisplayAllStatuses { get => displayedJobStatus == JobStatus.None; }
        public bool DisplayQueuedJobs { get => displayedJobStatus == JobStatus.Queued; }
        public bool DisplayReadyJobs { get => displayedJobStatus == JobStatus.Ready; }
        public bool DisplayRunningJobs { get => displayedJobStatus == JobStatus.Running; }
        public bool DisplaySuccessfulJobs { get => displayedJobStatus == JobStatus.Success; }
        public bool DisplaySuccessWithErrorJobs { get => displayedJobStatus == JobStatus.SuccessWithErrors; }
        public bool DisplayErroredJobs { get => displayedJobStatus == JobStatus.Error; }


        #endregion

        #region Private methods
        private void LoadJobs()
        {
            JobInfoList.Clear();

            try
            {
                IEnumerable<JobInfo> jobList = jobRepository.GetJobInfoByCriteria(displayedJobType, displayedJobStatus);

                foreach (JobInfo jobInfo in jobList)
                    JobInfoList.Add(jobInfo);
            }
            catch (Exception ex)
            {
                alertDialogViewModel.Title = "Problem loading jobs"; ;
                alertDialogViewModel.Message = $"A problem was encountered when loading the jobs: {ex.Message}";
                windowService.ShowDialog(alertDialogViewModel);
            }
            RaisePropertyChanged(() => CanCancelSelectedJobs);
        }

        private void ViewJobDetails()
        {
            jobDetailViewModel.JobID = SelectedJobInfo.JobID;
            windowService.ShowDialog(jobDetailViewModel);
            jobDetailViewModel.Clear();
        }

 


        private void DisplayJobType(JobType jobType)
        {
            displayedJobType = jobType;
            LoadJobs();

            RaisePropertyChanged(() => DisplayAllTypes);
            RaisePropertyChanged(() => DisplayFileSourceFolderScanJob);
        }

        private void DisplayJobStatus(JobStatus status)
        {
            displayedJobStatus = status;
            LoadJobs();

            RaisePropertyChanged(() => DisplayAllStatuses);
            RaisePropertyChanged(() => DisplayQueuedJobs);
            RaisePropertyChanged(() => DisplayReadyJobs);
            RaisePropertyChanged(() => DisplayRunningJobs);
            RaisePropertyChanged(() => DisplaySuccessfulJobs);
            RaisePropertyChanged(() => DisplaySuccessWithErrorJobs);
            RaisePropertyChanged(() => DisplayErroredJobs);
        }

        private void RestartSelectedJobs()
        {
            alertDialogViewModel.Title = "Restart Selected Jobs"; ;
            alertDialogViewModel.Message = $"Attempt to restart: {SelectedJobInfo?.JobID}";
            windowService.ShowDialog(alertDialogViewModel);
        }

        private void CancelSelectedJobs()
        {
            alertDialogViewModel.Title = "Cancel Selected Jobs"; ;
            alertDialogViewModel.Message = $"Attempt to cancel: {SelectedJobInfo?.JobID}";
            windowService.ShowDialog(alertDialogViewModel);
        }

        private void RemoveSelectedJobs()
        {
            alertDialogViewModel.Title = "Remove Selected Jobs"; ;
            alertDialogViewModel.Message = $"Attempt to remove: {SelectedJobInfo?.JobID}";
            windowService.ShowDialog(alertDialogViewModel);
        }



        private void JobMonitor_JobSubmitted(object sender, JobChangeEventArgs e)
        {
            JobInfo jobInfo = jobRepository.GetJobInfo(e.JobID);

            if ((displayedJobStatus == JobStatus.None || jobInfo.Status == displayedJobStatus) &&
                (displayedJobType == JobType.None || jobInfo.JobType == displayedJobType))
                JobInfoList.Insert(0, jobInfo);
                
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
        }

        #endregion
    }
}