using Dflat.Application.Services;

namespace DflatCoreWPF.ViewModels
{
    public class JobMonitorViewModel : ViewModelBase
    {
        private readonly JobMonitor jobMonitor;

        public JobMonitorViewModel(JobMonitor jobMonitor)
        {
            this.jobMonitor = jobMonitor;
        }
    }
}
