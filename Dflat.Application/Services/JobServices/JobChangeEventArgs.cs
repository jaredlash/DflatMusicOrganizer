﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dflat.Application.Services.JobServices
{
    public class JobChangeEventArgs : EventArgs
    {
        public enum JobChangeType
        {
            Submitted,
            Started,
            Updated,
            Finished
        }

        public int JobID { get; set; }
        public JobChangeType ChangeType { get; set; }
    }
}
