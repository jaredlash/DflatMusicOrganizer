using Dflat.Business.Factories;
using Dflat.Business.Models;
using Dflat.Business.Repositories;
using Dflat.Infrastructure.IO.Filesystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dflat.Business.Services
{
    public class FileSourceFolderScanService : JobService<FileSourceFolderScanJob>, IJobService<FileSourceFolderScanJob>
    {


        private readonly IFolderSearchService folderScanner;
        private readonly IJobService<FileMD5Job> fileMD5Service;
        private readonly IJobService<FileChromaprintJob> fileChromaprintService;

        private HashSet<string> validExtensions;

        public FileSourceFolderScanService(IUnitOfWorkFactory unitOfWorkFactory,
                                           IJobQueue jobQueue,
                                           IBackgroundJobRunner<FileSourceFolderScanJob> jobRunner,
                                           IJobService<FileMD5Job> fileMD5Service,
                                           IJobService<FileChromaprintJob> fileChromaprintService,
                                           IFolderSearchService folderScanner)
            : base(unitOfWorkFactory, jobQueue, jobRunner)
        {
            this.fileMD5Service = fileMD5Service;
            this.fileChromaprintService = fileChromaprintService;
            this.folderScanner = folderScanner;

            validExtensions = new HashSet<string>() { ".aiff", ".flac", ".m4a", ".mp2", ".mp3", ".ogg", ".wav", ".wma" };


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
            FolderSearchServiceResult result;
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

            var separator = new string(Path.DirectorySeparatorChar, 1);
            foreach (var fileResult in result.FoundFiles)
            {
                int newFileID;
                string fullFilePath;

                using (var unitOfWork = unitOfWorkFactory.Create())
                {
                    // Create the File object if it doesn't already exist
                    var newFile = fileResult.CreateFile();

                    try
                    {
                        unitOfWork.FileRepository.Add(newFile);
                    }
                    catch (DuplicateFileException)
                    {
                        // If this file already exists, just proceed to the next file
                        continue;
                    }
                    unitOfWork.SaveChanges();

                    newFileID = newFile.FileID;
                    fullFilePath = string.Join(separator, newFile.Directory, newFile.Filename);
                }


                // Queue a MD5 and Chromaprint requests
                fileChromaprintService.SubmitJobRequest(new FileChromaprintJob { FileID = newFileID, Description = "Chromaprint: " + fullFilePath });
                fileMD5Service.SubmitJobRequest(new FileMD5Job { FileID = newFileID, Description = "MD5: " + fullFilePath });

            }
        }

        private FolderSearchServiceResult ScanFolder(FileSourceFolderScanJob job)
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
