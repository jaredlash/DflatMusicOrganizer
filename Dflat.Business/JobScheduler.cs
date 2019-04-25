using Dflat.Business.Factories;
using Dflat.Business.Models;
using Dflat.Business.Services;
using System;

namespace Dflat.Jobs
{
	public class JobScheduler
	{
        private IJobService<FileSourceFolderScanJob> fileSourceFolderScanService;
        private IJobService<FileMD5Job> fileMD5Service;


        private IUnitOfWorkFactory unitOfWorkFactory;

		public JobScheduler(IJobService<FileSourceFolderScanJob> fileSourceFolderScanService, IJobService<FileMD5Job> fileMD5Service, IUnitOfWorkFactory unitOfWorkFactory)
		{
            this.fileSourceFolderScanService = fileSourceFolderScanService;
            this.fileMD5Service = fileMD5Service;

            this.unitOfWorkFactory = unitOfWorkFactory;

            this.fileSourceFolderScanService.JobFinished += JobFinished;
            this.fileMD5Service.JobFinished += JobFinished;
		}

        public void Start()
        {
            // Reset jobs that were interrupted during the last run to a queued state


            // After resetting, begin jobs again
            RunJobs();
        }


        public void JobFinished(object o, EventArgs e)
        {
            var job = o as Job;


            RunJobs();
        }


        public void RunJobs()
        {
            this.fileSourceFolderScanService.RunJobs();
        }
	}
}

