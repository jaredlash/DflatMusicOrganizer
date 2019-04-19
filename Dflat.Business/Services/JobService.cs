using Dflat.Business.Factories;
using Dflat.Business.Models;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Business.Services
{
    public abstract class JobService<JobType> : ViewModelBase where JobType : Job
    {
        public int MaxConcurrentJobs { get; set; }
        public int RunningJobCount { get; private set; }

        private IJobQueue<JobType> jobQueue;

        protected readonly IUnitOfWorkFactory unitOfWorkFactory;

        public JobService(IUnitOfWorkFactory unitOfWorkFactory, IJobQueue<JobType> jobQueue)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.jobQueue = jobQueue;

            RunningJobCount = 0;
        }


        public void RunJobs()
        {
            while (true)
            {
                if (RunningJobCount == MaxConcurrentJobs)
                    return;

                var job = jobQueue.GetNextAvailableJob();
                if (job == null)
                    return;

                // Do Async timer here for throttling jobs (such as with the AcoustID and MusicBrainz cases)
                
                SetupJob(job);

                DoWork(job);
                // ContinueWithSynchronizationContext...
                FinishJob(job);

            }
        }


        public void SubmitJobRequest(JobType job)
        {
            QueuePrerequisites(job);

            jobQueue.Add(job);
        }

        public abstract void QueuePrerequisites(JobType job);

        public abstract void SetupJob(JobType job);

        public abstract void DoWork(JobType job);
        
        public virtual void FinishJob(JobType job)
        {

            // Let things know we're done.
            if (JobFinished != null)
                JobFinished(job, null);
        }
        

        public event EventHandler JobFinished;
    }
}
