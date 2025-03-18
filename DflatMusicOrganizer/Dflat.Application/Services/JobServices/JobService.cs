using Dflat.Application.Models;
using Dflat.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dflat.Application.Services.JobServices
{
    public abstract class JobService<JobType> : IJobService<JobType> where JobType : Job
    {
        private readonly IJobRepository jobRepository;
        private readonly IBackgroundJobRunner<JobType> jobRunner;

        private readonly Dictionary<int, CancellationTokenSource> jobCancellationTokenSources;

        public int MaxConcurrentJobs { get; set; }
        public int RunningJobCount { get; private set; }
        public bool EnableRunningJobs { get; set; }
        public ICollection<Type> AcceptedRequestTypes { get; private set; }

        public JobService(IJobRepository jobRepository, IBackgroundJobRunner<JobType> jobRunner)
        {
            this.jobRepository = jobRepository;
            this.jobRunner = jobRunner;

            this.jobRunner.BackgroundWork = DoWork;
            this.jobRunner.FinishWork = FinishJob;

            RunningJobCount = 0;
            AcceptedRequestTypes = new List<Type>();

            jobCancellationTokenSources = new Dictionary<int, CancellationTokenSource>();
            EnableRunningJobs = false; // Default to disabled
        }


        public List<Task> RunJobs()
        {
            var taskList = new List<Task>();

            while (true)
            {
                // Subclassed JobServices which need to make throttled HTTP connections, should
                // use a custom HttpClient which performs the throttling to ensure APIs that are rate-limited
                // don't get over-used (such as AcoustID and MusicBrainz)

                // Run jobs only if enabled
                if (EnableRunningJobs == false)
                    return taskList;


                // Limit number of concurrent jobs
                if (RunningJobCount == MaxConcurrentJobs)
                    return taskList;

                // See if we have any more jobs ready to run
                var job = jobRepository.GetNextAvailable<JobType>();
                if (job == null)
                    return taskList;

                // Found a job to run
                RunningJobCount++;

                // Allow cancelling the job if we know the ID
                var cancellationTokenSource = new CancellationTokenSource();
                jobCancellationTokenSources.Add(job.JobID, cancellationTokenSource);

                // Perform any setup operations before background processing
                SetupJob(job);

                // Notify that we are running this job
                JobChanged?.Invoke(this, new JobChangeEventArgs { JobID = job.JobID, ChangeType = JobChangeEventArgs.JobChangeType.Started });

                // Run the job on a background thread
                taskList.Add(jobRunner.Run(job, cancellationTokenSource.Token));
            }
        }


        public void SubmitJobRequest(JobType job)
        {
            // TODO: Should probably add the job first in a new state such as "Queueing"
            // That way the job has an ID that can be used when queueing the prerequisites
            // Reevaluate when implementing QueuePrerequisites for a subclass.
            QueuePrerequisites(job);

            // TODO: Re-implement prerequisites once actually used.
            //if (job.PrerequisiteJobs.Count == 0)
            //    job.Status = JobStatus.Ready;
            //else
            //    job.Status = JobStatus.Queued;
            job.Status = JobStatus.Ready;

            // Then we add (possibly update, given note above) the job from Queueing to Queued or Ready depending on how
            // many prerequisites were queued.
            // Consider getting rid of the Prerequisites collection and moving this
            // logic to the database layer (although, it is more of a business concern...)
            jobRepository.Add(job);
            JobChanged?.Invoke(this, new JobChangeEventArgs { JobID = job.JobID, ChangeType = JobChangeEventArgs.JobChangeType.Submitted });

            // Start running jobs as soon as we get them
            RunJobs();
        }

        public abstract void QueuePrerequisites(JobType job);

        /// <summary>
        /// Override to indicate steps the service must do for setup.  This happens on calling thread.
        /// </summary>
        /// <param name="job"></param>
        public virtual void SetupJob(JobType job)
        {
            job.Output = "";
            job.Errors = "";
        }

        /// <summary>
        /// Override to indicate the actual work for the job which must be done on a background thread.
        /// </summary>
        /// <param name="job"></param>
        /// <param name="cancellationToken"></param>
        public abstract void DoWork(JobType job, CancellationToken cancellationToken);

        /// <summary>
        /// Override to indicate work to finish the job.
        /// 
        /// This happens on the original thread (not the background thread).  Overriding classes should call
        /// the base class method as the last step in any overrides.
        /// </summary>
        /// <param name="job">Finishing job</param>
        /// <param name="cancellationToken"></param>
        public virtual void FinishJob(JobType job, CancellationToken cancellationToken)
        {
            // Last chance to cancel
            if (cancellationToken.IsCancellationRequested)
                job.Status = JobStatus.Cancelled;

            // Save the job's status, and update status of any jobs waiting on this one
            jobRepository.Update(job);

            // No longer running this job
            RunningJobCount--;

            // Dispose of our CancellationTokenSource
            if (jobCancellationTokenSources.ContainsKey(job.JobID))
            {
                var cancellationTokenSource = jobCancellationTokenSources[job.JobID];
                jobCancellationTokenSources.Remove(job.JobID);
                cancellationTokenSource.Dispose();
            }

            // Let things know we're done.
            var changeEventArgs = new JobChangeEventArgs { JobID = job.JobID };
            if (job.Status == JobStatus.Cancelled)
                changeEventArgs.ChangeType = JobChangeEventArgs.JobChangeType.Cancelled;
            else
                changeEventArgs.ChangeType = JobChangeEventArgs.JobChangeType.Finished;

            JobChanged?.Invoke(this, changeEventArgs);

            // Finished with a job means we can start running the next
            RunJobs();
        }

        /// <summary>
        /// Attempts to cancel a running job
        /// </summary>
        /// <param name="jobID">Job ID to cancel</param>
        /// <returns>True if JobService was currently running job</returns>
        public bool TryCancelJob(int jobID)
        {
            // If the job is currently running, cancel it.
            // FinishJob should know about the cancellation, so that additional jobs don't get scheduled based on this.
            if (jobCancellationTokenSources.ContainsKey(jobID))
            {
                var cancellationTokenSource = jobCancellationTokenSources[jobID];
                cancellationTokenSource.Cancel();
                return true;
            }

            return false;
        }


        public void CancelRunningJobs()
        {
            foreach (var cancellationTokenSource in jobCancellationTokenSources.Values)
                cancellationTokenSource.Cancel();
        }


        public event EventHandler<JobChangeEventArgs> JobChanged;
    }
}
