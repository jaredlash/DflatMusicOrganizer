using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dflat.Application.Services;
using DflatCoreWPF.WindowService;
using System.ComponentModel;

namespace DflatCoreWPF.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
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
        JobStatus = string.Empty;

        this.jobMonitor.PropertyChanged += JobMonitor_PropertyChanged;
    }


    #endregion

    #region Bindable Properties

    [ObservableProperty]
    private string jobStatus;

    #endregion


    #region Private command methods
    [RelayCommand]
    private void Initialize()
    {
        jobMonitor.StartProcessing();
    }

    [RelayCommand]
    private void OpenFileSourceManager()
    {
        windowService.ShowDialog(fileSourceManagerViewModel);
    }

    [RelayCommand]
    private void OpenJobsView()
    {
        windowService.ShowWindow(jobMonitorViewModel);
    }

    [RelayCommand]
    private void Closing(CancelEventArgs args)
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

    private void JobMonitor_PropertyChanged(object? sender, PropertyChangedEventArgs e)
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
