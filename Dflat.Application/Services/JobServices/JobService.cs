using Dflat.Application.Models;
using Dflat.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Application.Services.JobServices
{
    public abstract class JobService<JobType> : IJobService<JobType> where JobType : Job
    {
        private readonly IJobRepository jobRepository;
        private readonly IBackgroundJobRunner<JobType> jobRunner;

        public int MaxConcurrentJobs { get; set; }
        public int RunningJobCount { get; private set; }


        public JobService(IJobRepository jobRepository, IBackgroundJobRunner<JobType> jobRunner)
        {
            this.jobRepository = jobRepository;
            this.jobRunner = jobRunner;

            this.jobRunner.BackgroundWork = DoWork;
            this.jobRunner.FinishWork = FinishJob;
            RunningJobCount = 0;
        }


        public List<Task> RunJobs()
        {
            var taskList = new List<Task>();

            while (true)
            {
                // Subclassed JobServices which need to make throttled HTTP connections, should
                // use a custom HttpClient which performs the throttling to ensure APIs that are rate-limited
                // don't get over-used (such as AcoustID and MusicBrainz)

                if (RunningJobCount == MaxConcurrentJobs)
                    return taskList;

                var job = jobRepository.GetNextAvailable<JobType>();
                if (job == null)
                    return taskList;

                RunningJobCount++;

                SetupJob(job);

                taskList.Add(jobRunner.Run(job));
            }
        }


        public void SubmitJobRequest(JobType job)
        {
            // TODO: Should probably add the job first in a new state such as "Queueing"
            // That way the job has an ID that can be used when queueing the prerequisites
            // Reevaluate when implementing QueuePrerequisites for a subclass.
            QueuePrerequisites(job);


            if (job.PrerequisiteJobs.Count == 0)
                job.Status = JobStatus.Ready;
            else
                job.Status = JobStatus.Queued;

            // Then we add (possibly update, given note above) the job from Queueing to Queued or Ready depending on how
            // many prerequisites were queued.
            // Consider getting rid of the Prerequisites collection and moving this
            // logic to the database layer (although, it is more of a business concern...)
            jobRepository.Add(job);

            // Start running jobs as soon as we get them
            RunJobs();
        }

        public abstract void QueuePrerequisites(JobType job);

        /// <summary>
        /// Override to indicate steps the service must do for setup.  This happens on calling thread.
        /// </summary>
        /// <param name="job"></param>
        public abstract void SetupJob(JobType job);

        /// <summary>
        /// Override to indicate the actual work for the job which must be done on a background thread.
        /// </summary>
        /// <param name="job"></param>
        public abstract void DoWork(JobType job);

        /// <summary>
        /// Override to indicate work to finish the job.
        /// 
        /// This happens on the original thread (not the background thread).  Also, overriding classes should call
        /// the base class method as the last step in any overrides.
        /// </summary>
        /// <param name="job"></param>
        public virtual void FinishJob(JobType job)
        {
            // Save the job's status, and update status of any jobs waiting on this one
            jobRepository.Update(job);

            RunningJobCount--;

            // Let things know we're done.
            JobFinished?.Invoke(job, null);

            // Finished with a job means we can start running the next
            RunJobs();
        }


        public event EventHandler JobFinished;
    }
}
