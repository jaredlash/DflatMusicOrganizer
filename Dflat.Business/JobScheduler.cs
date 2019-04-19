using Dflat.Business.Factories;
using Dflat.Business.Models;
using Dflat.Business.Services;
using System;

namespace Dflat.Jobs
{
	public class JobScheduler
	{
        private FileSourceFolderScanService fileSourceFolderScanService;

        private IUnitOfWorkFactory unitOfWorkFactory;

		public JobScheduler(FileSourceFolderScanService fileSourceFolderScanService, IUnitOfWorkFactory unitOfWorkFactory)
		{
            this.fileSourceFolderScanService = fileSourceFolderScanService;

            this.unitOfWorkFactory = unitOfWorkFactory;
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

