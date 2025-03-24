using System;

namespace Dflat.Application.Models;

public abstract class Job
{
    public Job()
    {
        CreationTime = DateTime.Now;
        Status = JobStatus.Queued;
        Description = "";
        IgnoreCache = false;
        Output = "";
        Errors = "";
    }

    public int JobID { get; set; }
    public DateTime CreationTime { get; set; }
    public string Description { get; set; }
    public bool IgnoreCache { get; set; }
    public JobStatus Status { get; set; }
    public string Output { get; set; }
    public string Errors { get; set; }


    public void SetFromExisting(Job existingJob)
    {
        Status = existingJob.Status;
        Output = existingJob.Output;
        Errors = existingJob.Errors;
    }


    public abstract bool SameRequestAs(Job otherJob);
}
