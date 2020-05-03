using System;
using System.Collections.Generic;
using System.Text;

namespace Dflat.Application.Services.JobServices
{
    public class JobChangeEventArgs : EventArgs
    {
        public int JobID { get; set; }
    }
}
