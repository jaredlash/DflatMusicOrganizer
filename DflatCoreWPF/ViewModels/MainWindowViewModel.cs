using Caliburn.Micro;
using Dflat.Application.Models;
using Dflat.Application.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace DflatCoreWPF.ViewModels
{
    public class MainWindowViewModel : Screen
    {

        #region Private backing fields


        private readonly IWindowManager windowManager;
        private readonly FileSourceManagerViewModel fileSourceManagerViewModel;
        private readonly JobMonitorViewModel jobMonitorViewModel;
        private readonly JobMonitor jobMonitor;

        #endregion

        #region Constructor

        public MainWindowViewModel(IWindowManager windowManager,
                                   FileSourceManagerViewModel fileSourceManagerViewModel,
                                   JobMonitorViewModel jobMonitorViewModel,
                                   JobMonitor jobMonitor)
        {
            this.windowManager = windowManager;
            this.fileSourceManagerViewModel = fileSourceManagerViewModel;
            this.jobMonitorViewModel = jobMonitorViewModel;
            this.jobMonitor = jobMonitor;

            this.jobMonitor.PropertyChanged += JobMonitor_PropertyChanged;
        }


        #endregion

        #region public Commands

        public async Task OpenFileSourceManager()
        {
            await windowManager.ShowDialogAsync(fileSourceManagerViewModel, null, null);
        }

        public async Task OpenJobsView()
        {
            await windowManager.ShowWindowAsync(jobMonitorViewModel, null, null);
        }


        #endregion



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

        private string jobStatus;

        public string JobStatus
        {
            get { return jobStatus; }
            set
            {
                jobStatus = value;
                NotifyOfPropertyChange(() => JobStatus);
            }
        }

    }
}
