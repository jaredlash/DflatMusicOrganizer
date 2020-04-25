using Caliburn.Micro;
using Dflat.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace DflatCoreWPF.ViewModels
{
    public class JobMonitorViewModel : Screen
    {
        private readonly JobMonitor jobMonitor;

        public JobMonitorViewModel(JobMonitor jobMonitor)
        {
            this.jobMonitor = jobMonitor;
        }
    }
}
