﻿using Dflat.Application.Services;
using DflatCoreWPF.WindowService;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace DflatCoreWPF.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        #region Private backing fields
        private readonly IWindowService windowService;
        private readonly FileSourceManagerViewModel fileSourceManagerViewModel;
        private readonly JobMonitorViewModel jobMonitorViewModel;
        private readonly ConfirmShutdownViewModel confirmShutdownViewModel;
        private readonly JobMonitor jobMonitor;

        #endregion

        #region Constructor

        public MainWindowViewModel(IWindowService windowService,
                                   FileSourceManagerViewModel fileSourceManagerViewModel,
                                   JobMonitorViewModel jobMonitorViewModel,
                                   ConfirmShutdownViewModel confirmShutdownViewModel,
                                   JobMonitor jobMonitor)
        {
            this.windowService = windowService;
            this.fileSourceManagerViewModel = fileSourceManagerViewModel;
            this.jobMonitorViewModel = jobMonitorViewModel;
            this.confirmShutdownViewModel = confirmShutdownViewModel;
            this.jobMonitor = jobMonitor;

            this.jobMonitor.PropertyChanged += JobMonitor_PropertyChanged;
        }


        #endregion

        #region Bindable Properties
        private string jobStatus;

        public string JobStatus
        {
            get { return jobStatus; }
            set
            {
                jobStatus = value;
                RaisePropertyChanged(() => JobStatus);
            }
        }
        #endregion



        #region Public Commands
        public ICommand InitializeCommand { get => new RelayCommand(() => Initialize()); }
        public ICommand OpenFileSourceManagerCommand { get => new RelayCommand(() => OpenFileSourceManager()); }
        public ICommand OpenJobsViewCommand { get => new RelayCommand(() => OpenJobsView()); }
        public ICommand ClosingCommand { get => new RelayCommand<CancelEventArgs>((e) => OnClosing(e)); }

        #endregion


        #region Private command methods
        private void Initialize()
        {
            jobMonitor.StartProcessing();
        }

        private void OpenFileSourceManager()
        {
            windowService.ShowDialog(fileSourceManagerViewModel);
        }

        private void OpenJobsView()
        {
            windowService.ShowWindow(jobMonitorViewModel);
        }


        private void OnClosing(CancelEventArgs args)
        {
            if (jobMonitor.RunningJobCount > 0 ||
                (jobMonitor.ProcessingIsEnabled && jobMonitor.QueuedJobCount > 0))
            {
                var result = windowService.ShowDialog(confirmShutdownViewModel);

                if (result != true)
                {
                    args.Cancel = true;
                    return;
                }
            }
            
            args.Cancel = false;
        }
        #endregion



        #region Event handlers

        private void JobMonitor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "QueuedJobCount" || e.PropertyName == "RunningJobCount" || e.PropertyName == "FinishedJobCount")
            {
                JobStatus = $"Jobs: {jobMonitor.QueuedJobCount} queued, {jobMonitor.RunningJobCount} running, {jobMonitor.FinishedJobCount} finished";
            }
        }

        #endregion

        #region Public overrides
        public override void OnClose()
        {
            // Try to close any windows that might be open

            jobMonitorViewModel.TryClose();
        }

        #endregion
    }
}
