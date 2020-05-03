using AutoMapper;
using Dflat.Application.Models;
using Dflat.Application.Repositories;
using Dflat.EFCore.DB.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.EFCore.DB.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly IMapper mapper;

        public JobRepository(IMapper mapper)
        {
            this.mapper = mapper;
        }

        // Sets the ID of the passed in job
        public void Add<JobType>(JobType job) where JobType : Application.Models.Job
        {
            switch (job)
            {
                case Application.Models.FileSourceFolderScanJob folderScanJob:
                    Models.FileSourceFolderScanJob newJob = mapper.Map<Models.FileSourceFolderScanJob>(folderScanJob);

                    using (var context = new DataContext())
                    {
                        context.Add(newJob);
                        context.SaveChanges();
                    }

                    folderScanJob.JobID = newJob.JobID;
                    break;

                default:
                    throw new NotImplementedException();
            }

        }

        // TODO: Make this responsible for updating the status of any parent
        // jobs for which this job is a prerequisite
        public void Update<JobType>(JobType job) where JobType : Application.Models.Job
        {
            switch (job)
            {
                case Application.Models.FileSourceFolderScanJob folderScanJob:
                    //Models.FileSourceFolderScanJob newJob = mapper.Map<Models.FileSourceFolderScanJob>(folderScanJob);

                    using (var context = new DataContext())
                    {
                        Models.FileSourceFolderScanJob existingJob = context.FileSourceFolderScanJobs.Find(folderScanJob.JobID);
                        if (existingJob == null)
                            throw new Exception($"Job = {folderScanJob.JobID} not found");

                        //existingJob.CreationTime  // Should not change
                        existingJob.Status = folderScanJob.Status;
                        existingJob.Description = folderScanJob.Description;
                        existingJob.IgnoreCache = folderScanJob.IgnoreCache;
                        existingJob.Output = folderScanJob.Output;
                        existingJob.Errors = folderScanJob.Errors;
                        existingJob.FileSourceFolderID = folderScanJob.FileSourceFolderID;

                        context.SaveChanges();
                    }

                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public bool CancelJob(int jobID)
        {
            using var context = new DataContext();

            var job = context.Jobs.Find(jobID);

            if (job.Status != JobStatus.Success && job.Status != JobStatus.SuccessWithErrors)
                job.Status = JobStatus.Cancelled;
            else
                return false;

            try
            {
                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }


        // Sets the returned job to running statsu
        public JobType GetNextAvailable<JobType>() where JobType : Application.Models.Job
        {
            Type jobType = typeof(JobType);


            using (var context = new DataContext())
            {
                if (jobType == typeof(Application.Models.FileSourceFolderScanJob))
                {

                    // First get the jobs that are currently running so that we can check that the job we pick which is queued is not equivalent
                    // to one that is currently running.
                    var runningJobs = context.FileSourceFolderScanJobs.Where((j) => j.Status == JobStatus.Running).ToList();

                    // Now get the jobs that are ready to be run
                    var readyJobs = context.FileSourceFolderScanJobs.Where((j) => j.Status == JobStatus.Ready).ToList();

                    var nextJob = readyJobs.Where((j) => runningJobs.Any((r) => r.SameRequestAs(j)) == false).FirstOrDefault();

                    if (nextJob == null)
                        return null;

                    // Update job to running
                    nextJob.Status = JobStatus.Running;
                    context.SaveChanges();

                    return mapper.Map<Application.Models.FileSourceFolderScanJob>(nextJob) as JobType;
                }
                else
                    throw new NotImplementedException();
            }
        }


        public Application.Models.Job Get(int jobID)
        {
            using var context = new DataContext();

            var job = context.Jobs.Find(jobID);

            return job switch
            {
                Models.FileSourceFolderScanJob j => mapper.Map<Application.Models.FileSourceFolderScanJob>(j),
                _ => null,
            };
        }


        public JobInfo GetJobInfo(int jobID)
        {
            using var context = new DataContext();

            var job = context.Jobs.Find(jobID);
            return new JobInfo
            {
                JobID = job.JobID,
                JobType = JobTypeFromJobObject(job),
                CreationTime = job.CreationTime,
                Description = job.Description,
                Status = job.Status,
                IgnoreCache = job.IgnoreCache
            };
        }

        public IEnumerable<JobInfo> GetJobInfoByCriteria(JobType jobType = JobType.None, JobStatus status = JobStatus.None)
        {
            List<JobInfo> result = new List<JobInfo>();

            using (var context = new DataContext())
            {
                IQueryable<Models.Job> q;

                switch (jobType)
                {
                    case JobType.FileSourceFolderScanJob:
                        q = context.FileSourceFolderScanJobs;
                        break;

                    default:
                        q = context.Jobs;
                        break;
                }

                if (status != JobStatus.None)
                    q = q.Where((j) => j.Status == status);

                q = q.OrderByDescending((j) => j.CreationTime);

                result = q.Select((j) => new JobInfo
                                  {
                                      JobID = j.JobID,
                                      JobType = JobTypeFromJobObject(j),
                                      CreationTime = j.CreationTime,
                                      Description = j.Description,
                                      Status = j.Status,
                                      IgnoreCache = j.IgnoreCache
                                  }).ToList();
            }

            return result;
        }



        private static JobType JobTypeFromJobObject(Models.Job job)
        {
            return job switch
            {
                Models.FileSourceFolderScanJob _ => JobType.FileSourceFolderScanJob,
                _ => JobType.FileSourceFolderScanJob,
            };
        }



        //private List<JobType> GetCurrentlyRunning<JobType>() where JobType : Application.Models.Job
        //{
        //    return context.Jobs.AsNoTracking().OfType<JobType>().Where((j) => j.Status == JobStatus.Running).ToList();
        //}


        //private List<JobType> GetReadyJobs<JobType>() where JobType : Application.Models.Job
        //{
        //    return context.Jobs.AsNoTracking().OfType<JobType>().Where((j) => j.Status == JobStatus.Ready).ToList();
        //}
    }
}
