using Dflat.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dflat.Business.Services
{
    public class BackgroundJobRunner<JobType> : IBackgroundJobRunner<JobType> where JobType : Job
    {

        public void Run(JobType job)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                BackgroundWork(job);
            }, TaskCreationOptions.LongRunning).ContinueWith((t) =>
            {
                FinishWork(job);
            }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        public Action<JobType> BackgroundWork { get; set; }

        public Action<JobType> FinishWork { get; set; }
    }
}
