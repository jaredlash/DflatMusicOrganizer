using Dflat.Business;
using Dflat.Business.Models;
using Dflat.EF6.DataAccess;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.EF6.DataAccess.Integration_Tests
{
    [TestFixture]
    public class JobRepositoryTests : EFDataTest
    {
        JobRepository jobRepository;
        private int fileSourceFolderID;

        [SetUp]
        public override void TestInitialize()
        {
            base.TestInitialize();

            using (var unitOfWork = new UnitOfWork())
            {
                FileSourceFolder fileSourceFolder = new FileSourceFolder();
                fileSourceFolder.Path = "Z:\\";
                unitOfWork.FileSourceFolderRepository.Add(fileSourceFolder);

                unitOfWork.SaveChanges();
                fileSourceFolderID = fileSourceFolder.FileSourceFolderID;
            }

            jobRepository = new JobRepository(new DataContext());
        }


        #region Adding jobs

        [Test]
        public void AddAndSave_AddsJobToRepositoryWithNewJobID()
        {
            var folderScanJob = new FileSourceFolderScanJob(fileSourceFolderID);

            jobRepository.Add(folderScanJob);

            jobRepository.Save();

            Assert.AreNotEqual(0, folderScanJob.JobID);
        }

        #endregion


        #region Updating jobs

        #endregion


        #region Retrieving jobs

        [Test]
        public void GetNextAvailabeJobShouldReturnSavedJob_WhenAddAndSaveJobHasNoPrerequisites()
        {
            int folderScanJobID;


            var folderScanJob = new FileSourceFolderScanJob(fileSourceFolderID);
            folderScanJob.Status = JobStatus.Ready;
            jobRepository.Add(folderScanJob);

            jobRepository.Save();

            folderScanJobID = folderScanJob.JobID;


            FileSourceFolderScanJob job;

            job = jobRepository.GetNextAvailable<FileSourceFolderScanJob>();

            Assert.NotNull(job);
            Assert.AreEqual(folderScanJobID, job.JobID);
            Assert.AreEqual(JobStatus.Running, job.Status);
        }


        [Test]
        public void GetCurrentlyRunning_WhenThereAreThreeJobsWithOneRunning_ReturnsOneJobInAList()
        {
            int runningJobID;

            var folderScanJob = new FileSourceFolderScanJob(fileSourceFolderID);
            folderScanJob.Status = JobStatus.Running;
            jobRepository.Add(folderScanJob);
            jobRepository.Save();

            runningJobID = folderScanJob.JobID;

            folderScanJob = new FileSourceFolderScanJob(fileSourceFolderID);
            folderScanJob.Status = JobStatus.Ready;
            jobRepository.Add(folderScanJob);

            folderScanJob = new FileSourceFolderScanJob(fileSourceFolderID);
            folderScanJob.Status = JobStatus.Success;
            jobRepository.Add(folderScanJob);

            jobRepository.Save();

            // Run

            var runningJobs = jobRepository.GetCurrentlyRunning<FileSourceFolderScanJob>();

            // Verify
            Assert.AreEqual(1, runningJobs.Count);
            Assert.IsTrue(runningJobs.Any((j) => j.JobID == runningJobID));
        }


        [Test]
        public void GetReadyJobs_WhenThereAreThreeJobsWithOneReady_ReturnsOneJobInAList()
        {
            int readyJobID;

            var folderScanJob = new FileSourceFolderScanJob(fileSourceFolderID);
            folderScanJob.Status = JobStatus.Running;
            jobRepository.Add(folderScanJob);
            jobRepository.Save();

            readyJobID = folderScanJob.JobID;

            folderScanJob = new FileSourceFolderScanJob(fileSourceFolderID);
            folderScanJob.Status = JobStatus.Ready;
            jobRepository.Add(folderScanJob);

            folderScanJob = new FileSourceFolderScanJob(fileSourceFolderID);
            folderScanJob.Status = JobStatus.Success;
            jobRepository.Add(folderScanJob);

            jobRepository.Save();

            // Run

            var readyJobs = jobRepository.GetCurrentlyRunning<FileSourceFolderScanJob>();

            // Verify
            Assert.AreEqual(1, readyJobs.Count);
            Assert.IsTrue(readyJobs.Any((j) => j.JobID == readyJobID));
        }



        // public JobType GetNextAvailable<JobType>() where JobType : Job
        [Test]
        public void GetNextAvailable_WhenThereAreThreeJobsWithOneReady_And_ReadyJobIsEquivalentToRunningJob_ReturnsNull()
        {
            var folderScanJob = new FileSourceFolderScanJob(fileSourceFolderID);
            folderScanJob.Status = JobStatus.Running;
            jobRepository.Add(folderScanJob);

            folderScanJob = new FileSourceFolderScanJob(fileSourceFolderID);
            folderScanJob.Status = JobStatus.Ready;
            jobRepository.Add(folderScanJob);

            folderScanJob = new FileSourceFolderScanJob(fileSourceFolderID);
            folderScanJob.Status = JobStatus.Success;
            jobRepository.Add(folderScanJob);

            jobRepository.Save();

            // Run
            var nextJob = jobRepository.GetNextAvailable<FileSourceFolderScanJob>();

            // Verify
            Assert.IsNull(nextJob);
        }

        #endregion

        #region Querying jobs for prerequisites


        [Test]
        public void PrerequisitesFinished_WhenPrerequisitesAreNotFinishedWithSuccess_ReturnsFalse()
        {
            int jobID;

            var folderScanJob1 = new FileSourceFolderScanJob(fileSourceFolderID);
            folderScanJob1.Status = JobStatus.Queued;
            jobRepository.Add(folderScanJob1);
            jobRepository.Save();

            var folderScanJob2 = new FileSourceFolderScanJob(fileSourceFolderID);
            folderScanJob2.Status = JobStatus.Success;
            folderScanJob2.DependentJob = folderScanJob1;
            jobRepository.Add(folderScanJob2);
            jobRepository.Save();

            var folderScanJob3 = new FileSourceFolderScanJob(fileSourceFolderID);
            folderScanJob3.Status = JobStatus.Running;
            folderScanJob3.DependentJob = folderScanJob1;
            jobRepository.Add(folderScanJob3);
            jobRepository.Save();

            jobID = folderScanJob1.JobID;


            // Run
            bool result = jobRepository.PrerequisitesFinished(jobID);

            // Verify
            Assert.IsFalse(result);
        }

        [Test]
        public void PrerequisitesFinished_WhenPrerequisitesAreFinishedWithSuccess_ReturnsTrue()
        {
            int jobID;

            var folderScanJob1 = new FileSourceFolderScanJob(fileSourceFolderID);
            folderScanJob1.Status = JobStatus.Queued;
            jobRepository.Add(folderScanJob1);
            jobRepository.Save();

            var folderScanJob2 = new FileSourceFolderScanJob(fileSourceFolderID);
            folderScanJob2.Status = JobStatus.Success;
            folderScanJob2.DependentJob = folderScanJob1;
            jobRepository.Add(folderScanJob2);
            jobRepository.Save();

            var folderScanJob3 = new FileSourceFolderScanJob(fileSourceFolderID);
            folderScanJob3.Status = JobStatus.Success;
            folderScanJob3.DependentJob = folderScanJob1;
            jobRepository.Add(folderScanJob3);
            jobRepository.Save();

            jobID = folderScanJob1.JobID;


            // Run
            bool result = jobRepository.PrerequisitesFinished(jobID);

            // Verify
            Assert.IsTrue(result);
        }

        [Test]
        public void PrerequisitesFinished_WhenThereAreNoPrerequisites_ReturnsTrue()
        {
            int jobID;

            var folderScanJob1 = new FileSourceFolderScanJob(fileSourceFolderID);
            folderScanJob1.Status = JobStatus.Queued;
            jobRepository.Add(folderScanJob1);
            jobRepository.Save();

            jobID = folderScanJob1.JobID;


            // Run
            bool result = jobRepository.PrerequisitesFinished(jobID);

            // Verify
            Assert.IsTrue(result);
        }


        #endregion

        #region Removing jobs


        #endregion

    }
}
