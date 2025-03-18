using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dflat.Application.Services.JobServices;
using System;
using System.Collections.Generic;
using System.Text;
using Dflat.Application.Repositories;
using AutoMapper;
using Dflat.Application.Models;
using Moq;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Threading;

namespace Dflat.Application.Services.JobServices.Tests
{
    [TestClass()]
    public class FileSourceFolderScanServiceTests
    {
        #region Constructor mocks
        readonly Mock<IFileSourceFolderRepository> fileSourceFolderRepositoryMock = new Mock<IFileSourceFolderRepository>();
        readonly Mock<IFileRepository> fileRepositoryMock = new Mock<IFileRepository>();
        readonly Mock<IFolderSearchService> folderScannerMock = new Mock<IFolderSearchService>();
        readonly Mock<IMapper> mapperMock = new Mock<IMapper>();
        readonly Mock<IFileCollectionCompare> comparerMock = new Mock<IFileCollectionCompare>();
        readonly Mock<IJobService<MD5Job>> md5Mock = new Mock<IJobService<MD5Job>>();
        readonly Mock<IJobRepository> jobRepositoryMock = new Mock<IJobRepository>();
        readonly Mock<IBackgroundJobRunner<FileSourceFolderScanJob>> jobRunnerMock = new Mock<IBackgroundJobRunner<FileSourceFolderScanJob>>();
        #endregion

        [TestInitialize]
        public void Initialize()
        {
            
        }

        private FileSourceFolderScanService CreateFileSourceFolderScanService()
        {
            var fileSourceFolderRepository = fileSourceFolderRepositoryMock.Object;
            var fileRepository = fileRepositoryMock.Object;
            var folderScanner = folderScannerMock.Object;
            var mapper = mapperMock.Object;
            var comparer = comparerMock.Object;
            var md5service = md5Mock.Object;
            var jobRepository = jobRepositoryMock.Object;
            var jobRunner = jobRunnerMock.Object;

            var service = new FileSourceFolderScanService(fileSourceFolderRepository, fileRepository, folderScanner, mapper, comparer, md5service, jobRepository, jobRunner)
            {
                EnableRunningJobs = true
            };

            return service;
        }


        [TestMethod]
        public void FileSourceFolderScanService_InitializesMaxConcurrentJobsToFive()
        {
            var scanService = CreateFileSourceFolderScanService();

            Assert.AreEqual(5, scanService.MaxConcurrentJobs);
        }


        [TestMethod]
        // Covers the case when the FileSourceFolderID in the Job request is not valid/not found in FileSourcefolderRepository
        public void SetupJob_ClearsOutputAndErrors_WhenCalled()
        {
            var job = new FileSourceFolderScanJob()
            {
                FileSourceFolderID = 1,
                Status = JobStatus.Ready,
                Output = "Not Empty",
                Errors = "Not Empty"
            };
            var scanService = CreateFileSourceFolderScanService();


            scanService.SetupJob(job);

            Assert.IsTrue(string.IsNullOrEmpty(job.Output));
            Assert.IsTrue(string.IsNullOrEmpty(job.Errors));
        }

        [TestMethod]
        // Covers the case when the FileSourceFolderID in the Job request is not valid/not found in FileSourcefolderRepository
        public void DoWork_ResultsInError_WhenFileSourceFolderNotFoundInRepo()
        {
            fileSourceFolderRepositoryMock.Setup(r => r.Get(It.IsAny<int>())).Returns<FileSourceFolder>(null);
            var job = new FileSourceFolderScanJob()
            {
                FileSourceFolderID = 1,
                Status = JobStatus.Running
            };

            var scanService = CreateFileSourceFolderScanService();
            scanService.DoWork(job, new CancellationToken());

            Assert.AreEqual(JobStatus.Error, job.Status);
            System.Text.RegularExpressions.Match m = Regex.Match(job.Errors, @"FileSouorceFolder with ID = [0-9]+ not found\.");
            Assert.IsTrue(m.Success);
        }

        [TestMethod]
        public void DoWork_ResultsInError_WhenFileSourceFolderNotFound()
        {
            fileSourceFolderRepositoryMock.Setup(r => r.Get(It.IsAny<int>())).Returns(new FileSourceFolder());
            folderScannerMock.Setup(s => s.FindFiles(It.IsAny<string>(), It.IsAny<HashSet<string>>(), It.IsAny<Predicate<string>>(), It.IsAny<CancellationToken>()))
                .Throws(new DirectoryNotFoundException());
            var job = new FileSourceFolderScanJob()
            {
                FileSourceFolderID = 1,
                Status = JobStatus.Running
            };

            var scanService = CreateFileSourceFolderScanService();
            scanService.DoWork(job, new CancellationToken());

            Assert.AreEqual(JobStatus.Error, job.Status);
        }

        [TestMethod]
        public void DoWork_ResultsInError_WhenFindFilesHasNoFilesButDoesHaveErrors()
        {
            FolderSearchServiceResult result = new FolderSearchServiceResult();
            result.ErrorLog.Add("Error");
            result.ErrorLog.Add("Error");
            fileSourceFolderRepositoryMock.Setup(r => r.Get(It.IsAny<int>())).Returns(new FileSourceFolder());
            folderScannerMock.Setup(s => s.FindFiles(It.IsAny<string>(), It.IsAny<HashSet<string>>(), It.IsAny<Predicate<string>>(), It.IsAny<CancellationToken>()))
                .Returns(result);
            var job = new FileSourceFolderScanJob()
            {
                FileSourceFolderID = 1,
                Status = JobStatus.Running
            };

            var scanService = CreateFileSourceFolderScanService();
            scanService.DoWork(job, new CancellationToken());

            Assert.AreEqual(JobStatus.Error, job.Status);
        }

        [TestMethod]
        public void DoWork_ResultsInSuccess_WhenNoFilesAndNoErrors()
        {
            FolderSearchServiceResult result = new FolderSearchServiceResult();
            FileCollectionCompare.CompareResult compareResult = new FileCollectionCompare.CompareResult();
            fileSourceFolderRepositoryMock.Setup(r => r.Get(It.IsAny<int>())).Returns(new FileSourceFolder());
            folderScannerMock.Setup(s => s.FindFiles(It.IsAny<string>(), It.IsAny<HashSet<string>>(), It.IsAny<Predicate<string>>(), It.IsAny<CancellationToken>()))
                .Returns(result);
            fileRepositoryMock.Setup(r => r.GetFromPath(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), true)).Returns(new List<Models.File>());
            comparerMock.Setup(c => c.Compare(It.IsAny<IEnumerable<Models.File>>(), It.IsAny<IEnumerable<Models.File>>()))
                .Returns(compareResult);
            var job = new FileSourceFolderScanJob()
            {
                FileSourceFolderID = 1,
                Status = JobStatus.Running
            };

            var scanService = CreateFileSourceFolderScanService();
            scanService.DoWork(job, new CancellationToken());

            Assert.AreEqual(JobStatus.Success, job.Status);
        }

        [TestMethod]
        public void DoWork_ResultsInSuccessWithErrors_WhenFindFilesHasFilesAndErrors()
        {
            FolderSearchServiceResult result = new FolderSearchServiceResult();
            FileCollectionCompare.CompareResult compareResult = new FileCollectionCompare.CompareResult();
            result.ErrorLog.Add("Error");
            result.ErrorLog.Add("Error");
            result.FoundFiles.Add(new FileResult("", "", "", 0, DateTime.Now));
            fileSourceFolderRepositoryMock.Setup(r => r.Get(It.IsAny<int>())).Returns(new FileSourceFolder());
            folderScannerMock.Setup(s => s.FindFiles(It.IsAny<string>(), It.IsAny<HashSet<string>>(), It.IsAny<Predicate<string>>(), It.IsAny<CancellationToken>()))
                .Returns(result);
            fileRepositoryMock.Setup(r => r.GetFromPath(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), true)).Returns(new List<Models.File>());
            comparerMock.Setup(c => c.Compare(It.IsAny<IEnumerable<Models.File>>(), It.IsAny<IEnumerable<Models.File>>()))
                .Returns(compareResult);
            var job = new FileSourceFolderScanJob()
            {
                FileSourceFolderID = 1,
                Status = JobStatus.Running
            };

            var scanService = CreateFileSourceFolderScanService();
            scanService.DoWork(job, new CancellationToken());

            Assert.AreEqual(JobStatus.SuccessWithErrors, job.Status);
            Assert.AreEqual(2, job.Errors.Split('\n').Length);
        }


        [TestMethod]
        public void DoWork_CallsCompareWithTwoFiles_WhenTwoFilesAreFound()
        {
            FolderSearchServiceResult result = new FolderSearchServiceResult();
            FileCollectionCompare.CompareResult compareResult = new FileCollectionCompare.CompareResult();
            result.FoundFiles.Add(new FileResult("", "", "", 0, DateTime.Now));
            result.FoundFiles.Add(new FileResult("", "", "", 0, DateTime.Now));
            fileSourceFolderRepositoryMock.Setup(r => r.Get(It.IsAny<int>())).Returns(new FileSourceFolder());
            folderScannerMock.Setup(s => s.FindFiles(It.IsAny<string>(), It.IsAny<HashSet<string>>(), It.IsAny<Predicate<string>>(), It.IsAny<CancellationToken>()))
                .Returns(result);
            fileRepositoryMock.Setup(r => r.GetFromPath(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), true)).Returns(new List<Models.File>());
            comparerMock.Setup(c => c.Compare(It.IsAny<IEnumerable<Models.File>>(), It.IsAny<IEnumerable<Models.File>>()))
                .Returns(compareResult);
            var job = new FileSourceFolderScanJob()
            {
                FileSourceFolderID = 1,
                Status = JobStatus.Running
            };

            var scanService = CreateFileSourceFolderScanService();
            scanService.DoWork(job, new CancellationToken());

            comparerMock.Verify(c => c.Compare(It.IsAny<IEnumerable<Models.File>>(), It.Is<IEnumerable<Models.File>>(l => l.Count() == 2)), Times.Once);
        }


        [TestMethod]
        public void DoWork_CallsMarkRemovedTwiceWithTwoFiles_WhenTwoFilesAreRemoved()
        {
            FolderSearchServiceResult result = new FolderSearchServiceResult();
            FileCollectionCompare.CompareResult compareResult = new FileCollectionCompare.CompareResult();
            compareResult.Removed.Add(new Models.File());
            compareResult.Removed.Add(new Models.File());
            fileSourceFolderRepositoryMock.Setup(r => r.Get(It.IsAny<int>())).Returns(new FileSourceFolder());
            folderScannerMock.Setup(s => s.FindFiles(It.IsAny<string>(), It.IsAny<HashSet<string>>(), It.IsAny<Predicate<string>>(), new CancellationToken()))
                .Returns(result);
            fileRepositoryMock.Setup(r => r.GetFromPath(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), true)).Returns(new List<Models.File>());
            comparerMock.Setup(c => c.Compare(It.IsAny<IEnumerable<Models.File>>(), It.IsAny<IEnumerable<Models.File>>()))
                .Returns(compareResult);
            var job = new FileSourceFolderScanJob()
            {
                FileSourceFolderID = 1,
                Status = JobStatus.Running
            };

            var scanService = CreateFileSourceFolderScanService();
            scanService.DoWork(job, new CancellationToken());

            fileRepositoryMock.Verify(f => f.MarkRemoved(It.IsAny<int>()), Times.Exactly(2));
        }

        [TestMethod]
        public void DoWork_CallsAddFileTwiceWithTwoFiles_WhenTwoFilesAreAdded()
        {
            FolderSearchServiceResult result = new FolderSearchServiceResult();
            FileCollectionCompare.CompareResult compareResult = new FileCollectionCompare.CompareResult();
            compareResult.Added.Add(new Models.File());
            compareResult.Added.Add(new Models.File());
            fileSourceFolderRepositoryMock.Setup(r => r.Get(It.IsAny<int>())).Returns(new FileSourceFolder());
            folderScannerMock.Setup(s => s.FindFiles(It.IsAny<string>(), It.IsAny<HashSet<string>>(), It.IsAny<Predicate<string>>(), new CancellationToken()))
                .Returns(result);
            fileRepositoryMock.Setup(r => r.GetFromPath(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), true)).Returns(new List<Models.File>());
            comparerMock.Setup(c => c.Compare(It.IsAny<IEnumerable<Models.File>>(), It.IsAny<IEnumerable<Models.File>>()))
                .Returns(compareResult);
            var job = new FileSourceFolderScanJob()
            {
                FileSourceFolderID = 1,
                Status = JobStatus.Running
            };

            var scanService = CreateFileSourceFolderScanService();
            scanService.DoWork(job, new CancellationToken());

            fileRepositoryMock.Verify(f => f.Add(It.IsAny<Models.File>()), Times.Exactly(2));
        }

        [TestMethod]
        public void DoWork_CallsUpdateFileTwiceWithTwoFiles_WhenTwoFilesAreUpdated()
        {
            FolderSearchServiceResult result = new FolderSearchServiceResult();
            FileCollectionCompare.CompareResult compareResult = new FileCollectionCompare.CompareResult();
            compareResult.Modified.Add(new Models.File());
            compareResult.Modified.Add(new Models.File());
            fileSourceFolderRepositoryMock.Setup(r => r.Get(It.IsAny<int>())).Returns(new FileSourceFolder());
            folderScannerMock.Setup(s => s.FindFiles(It.IsAny<string>(), It.IsAny<HashSet<string>>(), It.IsAny<Predicate<string>>(), It.IsAny<CancellationToken>()))
                .Returns(result);
            fileRepositoryMock.Setup(r => r.GetFromPath(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), true)).Returns(new List<Models.File>());
            comparerMock.Setup(c => c.Compare(It.IsAny<IEnumerable<Models.File>>(), It.IsAny<IEnumerable<Models.File>>()))
                .Returns(compareResult);
            var job = new FileSourceFolderScanJob()
            {
                FileSourceFolderID = 1,
                Status = JobStatus.Running
            };

            var scanService = CreateFileSourceFolderScanService();
            scanService.DoWork(job, new CancellationToken());

            fileRepositoryMock.Verify(f => f.Update(It.IsAny<Models.File>()), Times.Exactly(2));
        }

        //[TestMethod]
        //public void DoWork_SubmitsTwoMD5Requests_WhenOneFileAddedAndOneFileUpdatedd()
        //{
        //    FolderSearchServiceResult result = new FolderSearchServiceResult();
        //    FileCollectionCompare.CompareResult compareResult = new FileCollectionCompare.CompareResult();
        //    compareResult.Added.Add(new Models.File());
        //    compareResult.Modified.Add(new Models.File());
        //    fileSourceFolderRepositoryMock.Setup(r => r.Get(It.IsAny<int>())).Returns(new FileSourceFolder());
        //    folderScannerMock.Setup(s => s.FindFiles(It.IsAny<string>(), It.IsAny<HashSet<string>>(), It.IsAny<Predicate<string>>()))
        //        .Returns(result);
        //    fileRepositoryMock.Setup(r => r.GetFromPath(It.IsAny<string>(), true)).Returns(new List<Models.File>());
        //    comparerMock.Setup(c => c.Compare(It.IsAny<IEnumerable<Models.File>>(), It.IsAny<IEnumerable<Models.File>>()))
        //        .Returns(compareResult);
        //    var job = new FileSourceFolderScanJob()
        //    {
        //        FileSourceFolderID = 1,
        //        Status = JobStatus.Running
        //    };

        //    var scanService = CreateFileSourceFolderScanService();
        //    scanService.DoWork(job);

        //    // Verify that the MD5 service had two jobs submitted
        //}


        //[TestMethod]
        //public void DoWork_SubmitsTwoChromaprintRequests_WhenOneFileAddedAndOneFileUpdatedd()
        //{
        //    FolderSearchServiceResult result = new FolderSearchServiceResult();
        //    FileCollectionCompare.CompareResult compareResult = new FileCollectionCompare.CompareResult();
        //    compareResult.Added.Add(new Models.File());
        //    compareResult.Modified.Add(new Models.File());
        //    fileSourceFolderRepositoryMock.Setup(r => r.Get(It.IsAny<int>())).Returns(new FileSourceFolder());
        //    folderScannerMock.Setup(s => s.FindFiles(It.IsAny<string>(), It.IsAny<HashSet<string>>(), It.IsAny<Predicate<string>>()))
        //        .Returns(result);
        //    fileRepositoryMock.Setup(r => r.GetFromPath(It.IsAny<string>(), true)).Returns(new List<Models.File>());
        //    comparerMock.Setup(c => c.Compare(It.IsAny<IEnumerable<Models.File>>(), It.IsAny<IEnumerable<Models.File>>()))
        //        .Returns(compareResult);
        //    var job = new FileSourceFolderScanJob()
        //    {
        //        FileSourceFolderID = 1,
        //        Status = JobStatus.Running
        //    };

        //    var scanService = CreateFileSourceFolderScanService();
        //    scanService.DoWork(job);

        //    // Verify that the Chromaprint service had two jobs submitted
        //}
    }
}