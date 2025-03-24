using Dflat.Application.Services;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DflatCoreWPF.ViewModels
{
    public partial class ConfirmShutdownViewModel : ViewModelBase
    {
        private enum ShutdownChoice
        {
            None,
            FinishJobs,
            CancelJobs
        }

        
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

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string heading;

        [ObservableProperty]
        private string jobsDescription;

        [ObservableProperty]
        private string status;

        [ObservableProperty]
        private bool buttonsEnabled;


        [RelayCommand]
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

        [RelayCommand]
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

        [RelayCommand]
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
