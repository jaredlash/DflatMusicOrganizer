using Dflat.Application.Models;
using Dflat.Application.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Dflat.Application.Services.JobServices
{
    public class FileSourceFolderScanService : JobService<FileSourceFolderScanJob>, IJobService<FileSourceFolderScanJob>
    {
        private readonly IFileSourceFolderRepository fileSourceFolderRepository;
        private readonly IFileRepository fileRepository;
        private readonly IFolderSearchService folderScanner;
        private readonly IFileCollectionCompare comparer;
        private readonly IJobService<MD5Job> md5Service;
        private readonly HashSet<string> validExtensions;

        public FileSourceFolderScanService(IFileSourceFolderRepository fileSourceFolderRepository,
                                           IFileRepository fileRepository,
                                           IFolderSearchService folderScanner,
                                           IFileCollectionCompare comparer,
                                           IJobService<MD5Job> md5Service,
                                           IJobRepository jobRepository,
                                           IBackgroundJobRunner<FileSourceFolderScanJob> jobRunner)
            : base(jobRepository, jobRunner)
        {

            this.fileSourceFolderRepository = fileSourceFolderRepository;
            this.fileRepository = fileRepository;
            this.folderScanner = folderScanner;
            this.comparer = comparer;
            this.md5Service = md5Service;
            validExtensions = new HashSet<string>() { ".aiff", ".flac", ".m4a", ".mp2", ".mp3", ".ogg", ".wav", ".wma" };


            MaxConcurrentJobs = 5;
            AcceptedRequestTypes.Add(typeof(FileSourceFolderScanJob));
        }


        public override void QueuePrerequisites(FileSourceFolderScanJob job)
        {
            // This job type has no prerequisites
        }


        public override void DoWork(FileSourceFolderScanJob job, CancellationToken cancellationToken)
        {
            var fileSourceFolder = fileSourceFolderRepository.Get(job.FileSourceFolderID);
            if (fileSourceFolder == null)
            {
                job.Errors = $"FileSouorceFolder with ID = {job.FileSourceFolderID} not found.";
                job.Status = JobStatus.Error;

                return;
            }

            var excludeFolders = new HashSet<string>(fileSourceFolder.ExcludePaths.Select((p) => p.Path));
            fileSourceFolderRepository.UpdateLastScanTimeAsync(fileSourceFolder.FileSourceFolderID);

            FolderSearchServiceResult result;
            try
            {
                result = folderScanner.FindFiles(fileSourceFolder.Path, excludeFolders, MusicFilter, cancellationToken);
                if (result.ErrorLog.Count > 0 && result.FoundFiles.Count == 0)
                {
                    job.Status = cancellationToken.IsCancellationRequested ? JobStatus.Cancelled : JobStatus.Error;
                    return;
                }
                job.Errors = string.Join("\n", result.ErrorLog);
            }
            catch (DirectoryNotFoundException e)
            {
                job.Errors = e.Message;
                job.Status = JobStatus.Error;

                return;
            }


            job.Output = ProcessFoundFiles(job, fileSourceFolder.Path, excludeFolders, result, cancellationToken);

            if (result.ErrorLog.Count > 0)
                job.Status = JobStatus.SuccessWithErrors;
            else
                job.Status = JobStatus.Success;
        }

        public override void FinishJob(FileSourceFolderScanJob job, CancellationToken cancellationToken)
        {
            //Queue a MD5 and Chromaprint requests
            foreach (var file in job.FilesNeedingMD5s)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var fullFilePath = string.Join(Path.DirectorySeparatorChar, file.Directory, file.Filename);

                //fileChromaprintService.SubmitJobRequest(new FileChromaprintJob { FileID = file.FileID, Description = "Chromaprint: " + fullFilePath });
                md5Service.SubmitJobRequest(new MD5Job { FileID = file.FileID, Description = "MD5: " + fullFilePath });
            }

            base.FinishJob(job, cancellationToken); 
        }

        private string ProcessFoundFiles(FileSourceFolderScanJob job,
                                         string path,
                                         IEnumerable<string> excludeFolders,
                                         FolderSearchServiceResult result,
                                         CancellationToken cancellationToken)
        {
            StringBuilder output = new StringBuilder();

            try
            {
                // Set up our collections to compare
                var beforeSearch = fileRepository.GetFromPath(path, excludeFolders);        // "before" collection
                
                // Default to assuming the found files are the same size as the before files set, or 1000, whichever is greater.
                int foundFilesInitialCapacity = beforeSearch.Count();
                foundFilesInitialCapacity = (foundFilesInitialCapacity > 1000) ? foundFilesInitialCapacity : 1000;

                List<Models.File> foundFiles = new List<Models.File>(foundFilesInitialCapacity);     // "after" collection

                foreach (var fileResult in result.FoundFiles)
                {
                    Models.File newFile = fileResult.ToNewFile();

                    foundFiles.Add(newFile);
                }

                cancellationToken.ThrowIfCancellationRequested();

                // Figure out what has changed during since the last scan
                var compareResult = comparer.Compare(beforeSearch, foundFiles);


                foreach (var removedFile in compareResult.Removed)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    output.AppendLine($"Removed: {removedFile.Directory}{Path.DirectorySeparatorChar}{removedFile.Filename}");
                    fileRepository.MarkRemoved(removedFile.FileID);
                }


                foreach (var modifiedFile in compareResult.Modified)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    output.AppendLine($"Modified: {modifiedFile.Directory}{Path.DirectorySeparatorChar}{modifiedFile.Filename}");
                    fileRepository.Update(modifiedFile);
                }

                foreach (var addedFile in compareResult.Added)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    output.AppendLine($"Added: {addedFile.Directory}{Path.DirectorySeparatorChar}{addedFile.Filename}");
                    fileRepository.Add(addedFile);
                }

                // Determine which files need MD5s
                foreach (var file in fileRepository.GetFromPath(path, excludeFolders))
                {
                    if (string.IsNullOrEmpty(file.MD5Sum))
                    {
                        job.FilesNeedingMD5s.Add(file);
                    }
                }

                

                output.AppendLine("-----------------------------");
                output.AppendLine($"Added: {compareResult.Added.Count} files");
                output.AppendLine($"Removed: {compareResult.Removed.Count} files");
                output.AppendLine($"Modified: {compareResult.Modified.Count} files");
            }
            catch (OperationCanceledException)
            {
                output.AppendLine("-----------------------------");
                output.AppendLine("Job Cancelled");
            }

            return output.ToString();
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
    }
}
