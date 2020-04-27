using Dflat.Application.Models;
using Dflat.Application.Repositories;
using Dflat.Application.Services;
using DflatCoreWPF.WindowService;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DflatCoreWPF.ViewModels
{
    public class JobMonitorViewModel : ViewModelBase
    {
        private readonly JobMonitor jobMonitor;
        private readonly IJobRepository jobRepository;
        private readonly IWindowService windowService;
        private readonly AlertDialogViewModel alertDialogViewModel;

        public JobMonitorViewModel(JobMonitor jobMonitor,
                                   IJobRepository jobRepository,
                                   IWindowService windowService,
                                   AlertDialogViewModel alertDialogViewModel)
        {
            this.jobMonitor = jobMonitor;
            this.jobRepository = jobRepository;
            this.windowService = windowService;
            this.alertDialogViewModel = alertDialogViewModel;
            JobInfoList = new BindingList<JobInfo>();
        }


        private void Initialize()
        {
            JobInfoList.Clear();

            try
            {
                foreach (JobInfo jobInfo in jobRepository.GetAllJobInfo())
                {
                    JobInfoList.Add(jobInfo);
                }
            }
            catch (Exception ex)
            {
                alertDialogViewModel.Title = "Problem loading jobs"; ;
                alertDialogViewModel.Message = $"A problem was encountered when loading the jobs: {ex.Message}";
                windowService.ShowDialog(alertDialogViewModel);
            }
        }


        #region Commands

        public ICommand InitializeCommand { get => new RelayCommand(() => Initialize()); }


        #endregion


        #region Bindable Properties


        public BindingList<JobInfo> JobInfoList { get; private set; }


        #endregion

    }
}
