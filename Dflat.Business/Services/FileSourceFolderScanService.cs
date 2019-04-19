using Dflat.Business.Factories;
using Dflat.Business.Models;
using Dflat.Infrastructure.IO.Interfaces.Filesystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Business.Services
{
    public class FileSourceFolderScanService : JobService<FileSourceFolderScanJob>
    {
        
        
        private readonly IJobQueue jobQueue;
        private readonly IFolderSearchService folderScanner;


        private HashSet<string> validExtensions;

        public FileSourceFolderScanService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IJobQueue jobQueue,
            IFolderSearchService folderScanner
            ) 
            : base(unitOfWorkFactory, jobQueue)
        {
            this.jobQueue = jobQueue;
            this.folderScanner = folderScanner;

            validExtensions =  new HashSet<string>() { ".aiff", ".flac", ".m4a", ".mp2", ".mp3", ".ogg", ".wav", ".wma" };


            MaxConcurrentJobs = 5;
        }


        public override void QueuePrerequisites(FileSourceFolderScanJob job)
        {
            // This job type has no prerequisites
        }
        

        public override void SetupJob(FileSourceFolderScanJob job)
        {
            // This job type does not need setup
        }

        public override void DoWork(FileSourceFolderScanJob job)
        {
            IFolderSearchServiceResult result;
            try
            { 
                result = ScanFolder(job);
            }
            catch (DirectoryNotFoundException e)
            {
                job.Errors = e.Message;
                job.Status = JobStatus.Error;

                return;
            }

            job.Errors = string.Join("\n", result.ErrorLog);

            foreach(var filename in result.FoundFiles)
            {
                // Create the File object if it doesn't already exist

                // Queue a FileInfo request

            }
        }

        private IFolderSearchServiceResult ScanFolder(FileSourceFolderScanJob job)
        {
            var fileSourceFolder = job.FileSourceFolder;
            var excludeFolders = new HashSet<string>(fileSourceFolder.ExcludePaths.Select((p) => p.Path));

            return folderScanner.FindFiles(fileSourceFolder.Path, excludeFolders, MusicFilter);
        }

        private bool MusicFilter(string filename)
        {
            string extension;
            try
            {
                extension = Path.GetExtension(filename).ToLowerInvariant();
            }
            catch (ArgumentException)
            {
                return false;
            }

            if (validExtensions.Contains(extension))
                return true;

            return false;
        }


        public override void FinishJob(FileSourceFolderScanJob job)
        {

            base.FinishJob(job);
        }

    }
}
