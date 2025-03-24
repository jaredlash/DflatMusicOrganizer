using System;
using System.ComponentModel;

namespace Dflat.Application.Models;

/// <summary>
/// Contains Job information without the larger fields
/// 
/// Omits the "errors" and "output" fields.
/// </summary>
public class JobInfo : INotifyPropertyChanged
{
    private int _jobID;
    private JobType _jobType;
    private DateTime _creationTime;
    private string _description = string.Empty;
    private bool _ignoreCache;
    private JobStatus _status;

    public int JobID
    {
        get => _jobID;
        set
        {
            _jobID = value;
            CallPropertyChanged(nameof(JobID));
        }
    }
    public JobType JobType
    {
        get => _jobType;
        set
        {
            _jobType = value;
            CallPropertyChanged(nameof(JobType));
        }
    }
    public DateTime CreationTime
    {
        get => _creationTime;
        set
        {
            _creationTime = value;
            CallPropertyChanged(nameof(CreationTime));
        }
    }
    public string Description
    {
        get => _description;
        set
        {
            _description = value;
            CallPropertyChanged(nameof(Description));
        }
    }
    public bool IgnoreCache
    {
        get => _ignoreCache;
        set
        {
            _ignoreCache = value;
            CallPropertyChanged(nameof(IgnoreCache));
        }
    }
    public JobStatus Status
    {
        get => _status;
        set
        {
            _status = value;
            CallPropertyChanged(nameof(Status));
        }
    }


    public void SetFromExisting(JobInfo jobInfo)
    {
        JobID = jobInfo.JobID;
        JobType = jobInfo.JobType;
        CreationTime = jobInfo.CreationTime;
        Description = jobInfo.Description;
        IgnoreCache = jobInfo.IgnoreCache;
        Status = jobInfo.Status;
    }



    public event PropertyChangedEventHandler? PropertyChanged;
    private void CallPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
