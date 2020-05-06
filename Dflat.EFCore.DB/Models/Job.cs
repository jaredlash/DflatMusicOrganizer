using Dflat.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dflat.Data.EFCore.Models
{
    public abstract class Job
    {
        public int JobID { get; set; }
        public DateTime CreationTime { get; set; }
        public string Description { get; set; }
        public bool IgnoreCache { get; set; }

        public JobStatus Status { get; set; }
        public string Output { get; set; }
        public string Errors { get; set; }

        public abstract bool SameRequestAs(Job otherJob);
    }
}
