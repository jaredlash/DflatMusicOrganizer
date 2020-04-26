using Dflat.Application.Services;
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
        private readonly JobMonitor jobMonitor;

        #endregion

        #region Constructor

        public MainWindowViewModel(IWindowService windowService,
                                   FileSourceManagerViewModel fileSourceManagerViewModel,
                                   JobMonitorViewModel jobMonitorViewModel,
                                   JobMonitor jobMonitor)
        {
            this.windowService = windowService;
            this.fileSourceManagerViewModel = fileSourceManagerViewModel;
            this.jobMonitorViewModel = jobMonitorViewModel;
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
        public ICommand OpenFileSourceManagerCommand
        {
            get => new RelayCommand(() => OpenFileSourceManager());
        }


        public ICommand OpenJobsViewCommand
        {
            get => new RelayCommand(() => OpenJobsView());
        }



        #endregion


        #region Private command methods

        private void OpenFileSourceManager()
        {
            windowService.ShowDialog(fileSourceManagerViewModel);
        }

        private void OpenJobsView()
        {
            windowService.ShowWindow(jobMonitorViewModel);
        }
        #endregion



        #region Event handlers

        private void JobMonitor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "QueuedJobCount" || e.PropertyName == "RunningJobCount" || e.PropertyName == "FinishedJobCount")
            {
                var status = new List<string>();
                if (jobMonitor.QueuedJobCount > 0)
                    status.Add($"{jobMonitor.QueuedJobCount} queued");
                if (jobMonitor.RunningJobCount > 0)
                    status.Add($"{jobMonitor.RunningJobCount} running");
                if (jobMonitor.FinishedJobCount > 0)
                    status.Add($"{jobMonitor.FinishedJobCount} finished");

                JobStatus = "Jobs: " + string.Join(", ", status);
            }
        }

        #endregion

    }
}
