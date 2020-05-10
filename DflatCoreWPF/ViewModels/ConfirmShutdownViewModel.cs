using Dflat.Application.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DflatCoreWPF.ViewModels
{
    public class ConfirmShutdownViewModel : ViewModelBase
    {

        private string jobsDescription;
        private string title;
        private string status;
        private readonly JobMonitor jobMonitor;

        public ConfirmShutdownViewModel(JobMonitor jobMonitor)
        {
            this.jobMonitor = jobMonitor;

            this.jobMonitor.PropertyChanged += JobMonitor_PropertyChanged;
            Title = "";
            JobsDescription = "";
            Status = "";
        }


        public string Title
        {
            get => title;
            set
            {
                title = value;
                RaisePropertyChanged();
            }
        }

        public string JobsDescription
        {
            get => jobsDescription;
            set
            {
                jobsDescription = value;
                RaisePropertyChanged();
            }
        }

        public string Status
        {
            get => status;
            set
            {
                status = value;
                RaisePropertyChanged();
            }
        }


        public ICommand FinishCommand { get => new RelayCommand(async () => await Finish()); }
        public ICommand CancelRunningCommand { get => new RelayCommand(async () => await CancelRunning()); }
        public ICommand ContinueRunningCommand { get => new RelayCommand(() => ContinueRunning()); }

        private async Task Finish()
        {
            TryClose(true); // Temporary to allow the program cleanly shutdown
        }

        private async Task CancelRunning()
        {
        }


        private void ContinueRunning()
        {
            TryClose(false);
        }



        private void JobMonitor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "QueuedJobCount" || e.PropertyName == "RunningJobCount" || e.PropertyName == "FinishedJobCount")
            {
                var titleDescription = new List<string>();
                var longDescription = new List<string>();
                if (jobMonitor.QueuedJobCount > 0)
                {
                    titleDescription.Add($"{jobMonitor.QueuedJobCount} jobs queued");
                    longDescription.Add($"Jobs Currently Queued: {jobMonitor.QueuedJobCount}");
                }
                if (jobMonitor.RunningJobCount > 0)
                {
                    titleDescription.Add($"{jobMonitor.RunningJobCount} jobs running");
                    longDescription.Add($"Jobs Currently Running: {jobMonitor.RunningJobCount}");
                }

                Title = "Shutdown: " + string.Join(", ", titleDescription);
                JobsDescription = string.Join("\n", longDescription);
            }
        }
    }
}
