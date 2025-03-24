using System;

namespace Dflat.Application.Services.JobServices;

public class JobChangeEventArgs : EventArgs
{
    public enum JobChangeType
    {
        Submitted,
        Started,
        Updated,
        Finished,
        Cancelled
    }

    public int JobID { get; set; }
    public JobChangeType ChangeType { get; set; }
}
