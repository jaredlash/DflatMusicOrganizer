using AutoMapper;
using Dflat.Application.Models;
using Dflat.Application.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dflat.Application.Services.JobServices
{
    public class FileSourceFolderScanService : JobService<FileSourceFolderScanJob>, IJobService<FileSourceFolderScanJob>
    {
        public enum ProcessFileMode
        {
            Stage,
            Add
        }

        private readonly IFileSourceFolderRepository fileSourceFolderRepository;
        private readonly IFileRepository fileRepository;
        private readonly IFolderSearchService folderScanner;
        private readonly IMapper mapper;
        private readonly HashSet<string> validExtensions;

        public FileSourceFolderScanService(IFileSourceFolderRepository fileSourceFolderRepository,
                                           IFileRepository fileRepository,
                                           IFolderSearchService folderScanner,
                                           IMapper mapper,
                                           IJobRepository jobRepository,
                                           IBackgroundJobRunner<FileSourceFolderScanJob> jobRunner)
            : base(jobRepository, jobRunner)
        {

            this.fileSourceFolderRepository = fileSourceFolderRepository;
            this.fileRepository = fileRepository;
            this.folderScanner = folderScanner;
            this.mapper = mapper;
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


            foreach (var fileResult in result.FoundFiles)
            {
                StageFileResult(fileResult);
            }
        }

        public void StageFileResult(FileResult fileResult)
        {
            var separator = new string(Path.DirectorySeparatorChar, 1);

            int newFileID;
            string fullFilePath;

            // Create the File object if it doesn't already exist
            Models.File newFile = new Models.File();

            mapper.Map(fileResult, newFile);

            fileRepository.Stage(newFile);


            newFileID = newFile.FileID;
            fullFilePath = string.Join(separator, newFile.Directory, newFile.Filename);



            // Queue a MD5 and Chromaprint requests
            //fileChromaprintService.SubmitJobRequest(new FileChromaprintJob { FileID = newFileID, Description = "Chromaprint: " + fullFilePath });
            //fileMD5Service.SubmitJobRequest(new FileMD5Job { FileID = newFileID, Description = "MD5: " + fullFilePath });
        }

        public ProcessFileMode ShouldStageFile(Models.File newFile, Models.File existingFile)
        {
            // TODO: Make decision configurable based on user options
            // for example: perhaps all files should be staged until MD5s can be compared

            // If we don't have an existing file that might match, then just add the new one
            if (existingFile == null)
                return ProcessFileMode.Add;

            return ProcessFileMode.Stage;
        }

        private FolderSearchServiceResult ScanFolder(FileSourceFolderScanJob job)
        {
            var fileSourceFolder = fileSourceFolderRepository.Get(job.FileSourceFolderID);
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
