﻿using Dflat.Business.Factories;
using Dflat.Business.Models;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Business.Services
{
    public abstract class JobService<JobType> : ViewModelBase, IJobService<JobType> where JobType : Job
    {
        public int MaxConcurrentJobs { get; set; }
        public int RunningJobCount { get; private set; }

        // This is private to make inheritors use the SubmitJobRequest method
        private IJobQueue jobQueue;

        private IBackgroundJobRunner<JobType> jobRunner;

        protected readonly IUnitOfWorkFactory unitOfWorkFactory;

        public JobService(IUnitOfWorkFactory unitOfWorkFactory, IJobQueue jobQueue, IBackgroundJobRunner<JobType> jobRunner)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.jobQueue = jobQueue;
            this.jobRunner = jobRunner;

            this.jobRunner.BackgroundWork = new Action<JobType>(DoWork);

            RunningJobCount = 0;
        }


        public void RunJobs()
        {
            while (true)
            {
                // Do Async timer here for throttling jobs (suc as with the AcoustID and MusicBrainz cases)


                if (RunningJobCount == MaxConcurrentJobs)
                    return;

                var job = jobQueue.GetNextAvailableJob<JobType>();
                if (job == null)
                    return;
                
                SetupJob(job);

                jobRunner.Run(job);

            }
        }


        public void SubmitJobRequest(JobType job)
        {
            QueuePrerequisites(job);

            if (job.PrerequisiteJobs.Count == 0)
                job.Status = JobStatus.Ready;
            else
                job.Status = JobStatus.Queued;

            jobQueue.Add(job);
        }

        public abstract void QueuePrerequisites(JobType job);

        public abstract void SetupJob(JobType job);

        public abstract void DoWork(JobType job);

        public virtual void FinishJob(JobType job)
        {
            // Save the job's sttaus
            using (var unitOfWork = unitOfWorkFactory.Create())
            {
                var changeJob = unitOfWork.JobRepository.Get(job.JobID);
                changeJob.SetFromExisting(job);
                unitOfWork.SaveChanges();
            }

            if (job.Status == JobStatus.Success)
                UpdateDependentJobStatus(job);

            // Let things know we're done.
            if (JobFinished != null)
                JobFinished(job, null);

        }


        private void UpdateDependentJobStatus(JobType job)
        {
            if (job.DependentJobID == null)
                return;

            int parentID = (int)job.DependentJobID;

            using (var unitOfWork = unitOfWorkFactory.Create())
            {
                // See if our parent job is ready to be run
                if (unitOfWork.JobRepository.PrerequisitesFinished(parentID))
                {
                    var parentJob = unitOfWork.JobRepository.Get(parentID);
                    parentJob.Status = JobStatus.Ready;
                    unitOfWork.SaveChanges();
                }
            }
        }

        public event EventHandler JobFinished;
    }
}