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
        private enum ShutdownChoice
        {
            None,
            FinishJobs,
            CancelJobs
        }

        private string jobsDescription;
        private string title;
        private string heading;
        private string status;
        private bool buttonsEnabled;
        private ShutdownChoice shutdownChoice;
        private readonly JobMonitor jobMonitor;

        public ConfirmShutdownViewModel(JobMonitor jobMonitor)
        {
            this.jobMonitor = jobMonitor;

            this.jobMonitor.PropertyChanged += JobMonitor_PropertyChanged;
            Title = "";
            Heading = "";
            JobsDescription = "";
            Status = "";
            ButtonsEnabled = true;
            shutdownChoice = ShutdownChoice.None;
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

        public string Heading
        {
            get => heading;
            set
            {
                heading = value;
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

        public bool ButtonsEnabled
        {
            get => buttonsEnabled;
            set
            {
                buttonsEnabled = value;
                RaisePropertyChanged();
            }
        }


        public ICommand FinishCommand { get => new RelayCommand(() => Finish()); }
        public ICommand CancelRunningCommand { get => new RelayCommand(() => CancelRunning()); }
        public ICommand ContinueRunningCommand { get => new RelayCommand(() => ContinueRunning()); }

        private void Finish()
        {
            ButtonsEnabled = false;
            Status = "Status: Stopping job processing. Waiting for running jobs to finish.";
            shutdownChoice = ShutdownChoice.FinishJobs;
            jobMonitor.StopProcessing(cancelRunningJobs: false);

            // Close if no more jobs are running after stopping all processing.
            if (jobMonitor.RunningJobCount == 0)
                TryClose(true);
        }

        private void CancelRunning()
        {
            ButtonsEnabled = false;
            Status = "Status:  Stopping job processing. Cancelling running jobs.";
            shutdownChoice = ShutdownChoice.CancelJobs;
            jobMonitor.StopProcessing(cancelRunningJobs: true);

            // Close if no more jobs are running after stopping all processing.
            if (jobMonitor.RunningJobCount == 0)
                TryClose(true);
        }


        private void ContinueRunning()
        {
            TryClose(false);
        }



        private void JobMonitor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Close if we're shutting down and we're no longer running jobs
            if (shutdownChoice != ShutdownChoice.None &&
                e.PropertyName == "RunningJobCount" &&
                jobMonitor.RunningJobCount == 0)
                TryClose(true);


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
                if (longDescription.Count > 0)
                {
                    Heading = "There are jobs currently in progress.";
                    JobsDescription = string.Join("\n", longDescription);
                }
                else
                {
                    Heading = "All jobs have finished and no jobs are queued.";
                    JobsDescription = string.Empty;
                }
            }
        }
    }
}
