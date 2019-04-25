using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Business.Models
{
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

        // This group of fields will be updated by setting a job's status
        public JobStatus Status { get; set; }
        public string Output { get; set; }
        public string Errors { get; set; }

        public void SetFromExisting(Job existingJob)
        {
            Status = existingJob.Status;
            Output = existingJob.Output;
            Errors = existingJob.Errors;
        }


        public int? DependentJobID { get; set; }
        public Job DependentJob { get; set; }

        public ICollection<Job> PrerequisiteJobs { get; set; }

        public abstract bool SameRequestAs(Job otherJob);
    }
}
