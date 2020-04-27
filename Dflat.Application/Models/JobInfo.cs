using System;
using System.Collections.Generic;
using System.Text;

namespace Dflat.Application.Models
{
    /// <summary>
    /// Contains Job information without the larger fields
    /// 
    /// Omits the "errors" and "output" fields.
    /// </summary>
    public class JobInfo
    {
        public int JobID { get; set; }
        public JobType JobType { get; set; }
        public DateTime CreationTime { get; set; }
        public string Description { get; set; }
        public bool IgnoreCache { get; set; }
        public JobStatus Status { get; set; }
    }
}
