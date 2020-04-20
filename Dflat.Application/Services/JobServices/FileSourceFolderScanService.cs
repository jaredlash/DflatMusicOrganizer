using AutoMapper;
using Dflat.Application.Models;
using Dflat.Application.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dflat.Application.Services.JobServices
{
    public class FileSourceFolderScanService : JobService<FileSourceFolderScanJob>, IJobService<FileSourceFolderScanJob>
    {
        private readonly IFileSourceFolderRepository fileSourceFolderRepository;
        private readonly IFileRepository fileRepository;
        private readonly IFolderSearchService folderScanner;
        private readonly IMapper mapper;
        private readonly IFileCollectionCompare comparer;
        private readonly HashSet<string> validExtensions;

        public FileSourceFolderScanService(IFileSourceFolderRepository fileSourceFolderRepository,
                                           IFileRepository fileRepository,
                                           IFolderSearchService folderScanner,
                                           IMapper mapper,
                                           IFileCollectionCompare comparer,
                                           IJobRepository jobRepository,
                                           IBackgroundJobRunner<FileSourceFolderScanJob> jobRunner)
            : base(jobRepository, jobRunner)
        {

            this.fileSourceFolderRepository = fileSourceFolderRepository;
            this.fileRepository = fileRepository;
            this.folderScanner = folderScanner;
            this.mapper = mapper;
            this.comparer = comparer;
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
            var fileSourceFolder = fileSourceFolderRepository.Get(job.FileSourceFolderID);
            if (fileSourceFolder == null)
            {
                job.Errors = $"FileSouorceFolder with ID = {job.FileSourceFolderID} not found.";
                job.Status = JobStatus.Error;

                return;
            }

            var excludeFolders = new HashSet<string>(fileSourceFolder.ExcludePaths.Select((p) => p.Path));


            FolderSearchServiceResult result;
            try
            {
                result = folderScanner.FindFiles(fileSourceFolder.Path, excludeFolders, MusicFilter);
                job.Errors = string.Join("\n", result.ErrorLog);
                if (result.ErrorLog.Count > 0 && result.FoundFiles.Count == 0)
                {
                        job.Status = JobStatus.Error;
                        return;
                }
            }
            catch (DirectoryNotFoundException e)
            {
                job.Errors = e.Message;
                job.Status = JobStatus.Error;

                return;
            }


            job.Output = ProcessFoundFiles(fileSourceFolder.Path, result);

            if (result.ErrorLog.Count > 0)
                job.Status = JobStatus.SuccessWithErrors;
            else
                job.Status = JobStatus.Success;
        }

        private string ProcessFoundFiles(string path, FolderSearchServiceResult result)
        {
            StringBuilder output = new StringBuilder();

            // Set up our collections to compare
            var beforeSearch = fileRepository.GetFromPath(path);        // "before" collection
            List<Models.File> foundFiles = new List<Models.File>();     // "after" collection
            
            foreach (var fileResult in result.FoundFiles)
            {
                Models.File newFile = mapper.Map<Models.File>(fileResult);

                foundFiles.Add(newFile);
            }

            
            // Figure out what has changed during since the last scan
            var compareResult = comparer.Compare(beforeSearch, foundFiles);


            foreach (var removedFile in compareResult.Removed)
            {
                output.AppendLine($"Removed: {removedFile.Directory}{Path.DirectorySeparatorChar}{removedFile.Filename}");
                fileRepository.MarkRemoved(removedFile.FileID);
            }


            foreach (var modifiedFile in compareResult.Modified)
            {
                output.AppendLine($"Modified: {modifiedFile.Directory}{Path.DirectorySeparatorChar}{modifiedFile.Filename}");
                fileRepository.Update(modifiedFile);
            }

            foreach (var addedFile in compareResult.Added)
            {
                output.AppendLine($"Added: {addedFile.Directory}{Path.DirectorySeparatorChar}{addedFile.Filename}");
                fileRepository.Add(addedFile); // Sets the FileID of the added file
            }

            //Queue a MD5 and Chromaprint requests
            foreach (var file in compareResult.Added.Concat(compareResult.Modified))
            {
                var separator = new string(Path.DirectorySeparatorChar, 1);
               
                var fullFilePath = string.Join(separator, file.Directory, file.Filename);

                
                //fileChromaprintService.SubmitJobRequest(new FileChromaprintJob { FileID = file.FileID, Description = "Chromaprint: " + fullFilePath });
                //fileMD5Service.SubmitJobRequest(new FileMD5Job { FileID = fileID, Description = "MD5: " + fullFilePath });
            }

            output.AppendLine("-----------------------------");
            output.AppendLine($"Added: {compareResult.Added.Count} files");
            output.AppendLine($"Removed: {compareResult.Removed.Count} files");
            output.AppendLine($"Modified: {compareResult.Modified.Count} files");

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
