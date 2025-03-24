using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dflat.Application.Models;
using Dflat.Application.Repositories;
using System;
using System.ComponentModel;

namespace DflatCoreWPF.ViewModels;

public partial class JobDetailViewModel : ViewModelBase
{
    private readonly IJobRepository jobRepository;
    
    public JobDetailViewModel(IJobRepository jobRepository)
    {
        this.jobRepository = jobRepository;

        JobParameters = [];
    }

    #region Bindable properties

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(JobIDDisplay))]
    [NotifyPropertyChangedFor(nameof(Title))]
    private int jobID;

    public string JobIDDisplay
    {
        get => $"(JobID {JobID})";
    }

    public string Title
    {
        get => $"Details for {JobType} {JobIDDisplay}";
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Title))]
    private string jobType = string.Empty;

    [ObservableProperty]
    private DateTime creationTime;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private bool ignoreCache;

    [ObservableProperty]
    private string status = string.Empty;

    [ObservableProperty]
    private string output = string.Empty;

    [ObservableProperty]
    private string errors = string.Empty;

    public BindingList<string> JobParameters { get; private set; }

    #endregion

    #region Public methods
    public void Clear()
    {
        Status = string.Empty;
        Description = string.Empty;
        Output = string.Empty;
        Errors = string.Empty;

        JobParameters.Clear();
    }

    #endregion

    #region Private methods

    [RelayCommand]
    private void Initialize()
    {
        var job = jobRepository.Get(JobID);

        if (job is null)
            return;

        JobType = job.ToString() ?? string.Empty;
        CreationTime = job.CreationTime;
        Status = job.Status.ToString();
        Description = job.Description;
        IgnoreCache = job.IgnoreCache;
        Output = job.Output;
        Errors = job.Errors;

        switch (job)
        {
            case FileSourceFolderScanJob j:
                JobParameters.Add($"FileSourceFolderID: {j.FileSourceFolderID}");
                break;
        }
    }

    [RelayCommand]
    private void Close()
    {
        TryClose();
    }

    #endregion
}
