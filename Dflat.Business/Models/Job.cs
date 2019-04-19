using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Business.Models
{
    public class Job
    {
        public int JobID { get; set; }
        public DateTime CreationTime { get; set; }
        public JobStatus Status { get; set; }
        public string Description { get; set; }
        public bool IgnoreCache { get; set; }
        public string Output { get; set; }
        public string Errors { get; set; }
    }
}
