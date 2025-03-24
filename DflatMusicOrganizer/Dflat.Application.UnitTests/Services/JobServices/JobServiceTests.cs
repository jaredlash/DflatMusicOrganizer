using Dflat.Application.Models;
using Dflat.Application.Repositories;
using Dflat.Application.Services.JobServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Nito.AsyncEx;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dflat.Application.UnitTests.Services.JobServices;

[TestClass]
public class JobServiceTests
{
    #region Setup
    private Mock<IJobRepository> CreateMockJobRepository()
    {
        var repo = new Mock<IJobRepository>();


        return repo;
    }

    private Mock<IBackgroundJobRunner<TestJob>> CreateBackgroundJobRunner()
    {
        var runner = new Mock<IBackgroundJobRunner<TestJob>>();

        return runner;
    }

    #endregion

    #region SetupJob


    [TestMethod]
    // Covers the case when the FileSourceFolderID in the Job request is not valid/not found in FileSourcefolderRepository
    public void SetupJob_ClearsOutputAndErrors_WhenCalled()
    {
        var repo = CreateMockJobRepository();
        repo.Setup(r => r.GetNextAvailable<TestJob>()).Returns(() => null);
        var runner = CreateBackgroundJobRunner();
        var jobServiceMock = new Mock<JobService<TestJob>>(repo.Object, runner.Object)
        {
            CallBase = true
        };
        var jobService = jobServiceMock.Object;
        var job = new TestJob()
        {
            Status = JobStatus.Ready,
            Output = "Not Empty",
            Errors = "Not Empty"
        };

        jobService.SetupJob(job);

        Assert.IsTrue(string.IsNullOrEmpty(job.Output));
        Assert.IsTrue(string.IsNullOrEmpty(job.Errors));
    }

    #endregion

    #region RunJobs
    [TestMethod]
    public void RunJobs_Exits_WhenNoJobsAvailable()
    {
        var repo = CreateMockJobRepository();
        repo.Setup(r => r.GetNextAvailable<TestJob>()).Returns(() => null);
        var runner = CreateBackgroundJobRunner();
        var jobServiceMock = new Mock<JobService<TestJob>>(repo.Object, runner.Object);
        var jobService = jobServiceMock.Object;
        jobService.MaxConcurrentJobs = 5;
        jobService.EnableRunningJobs = true;

        // Test
        jobService.RunJobs();

        // Verify
        jobServiceMock.Verify(m => m.SetupJob(It.IsAny<TestJob>()), Times.Never());
    }

    [TestMethod]
    public void RunJobs_RunsTwice_WhenTwoJobsAvailable()
    {
        var repo = CreateMockJobRepository();
        var jobs = new List<TestJob> { new TestJob { JobID = 1 }, new TestJob { JobID = 2 } };
        var iter = jobs.GetEnumerator();
        repo.Setup(r => r.GetNextAvailable<TestJob>()).Returns(() => { iter.MoveNext(); return iter.Current; });
        var runner = CreateBackgroundJobRunner();
        var jobServiceMock = new Mock<JobService<TestJob>>(repo.Object, runner.Object);
        var jobService = jobServiceMock.Object;
        jobService.MaxConcurrentJobs = 5;
        jobService.EnableRunningJobs = true;

        // Test
        jobService.RunJobs();

        // Verify
        jobServiceMock.Verify(m => m.SetupJob(It.IsAny<TestJob>()), Times.Exactly(2));
        runner.Verify(m => m.Run(It.IsAny<TestJob>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [TestMethod]
    public void RunJobs_RunsTwice_WhenThreeJobsAvilableAndMaxJobsIsTwo()
    {
        var repo = CreateMockJobRepository();
        var jobs = new List<TestJob> { new TestJob { JobID = 1 }, new TestJob { JobID = 2 }, new TestJob { JobID = 3 } }; // Set up with 3 jobs
        var iter = jobs.GetEnumerator();
        repo.Setup(r => r.GetNextAvailable<TestJob>()).Returns(() => { iter.MoveNext(); return iter.Current; });
        var runner = CreateBackgroundJobRunner();
        var jobServiceMock = new Mock<JobService<TestJob>>(repo.Object, runner.Object);
        var jobService = jobServiceMock.Object;
        jobService.MaxConcurrentJobs = 2;  // Allow a maximum of two to run.
        jobService.EnableRunningJobs = true;

        // Test
        jobService.RunJobs();

        // Verify
        jobServiceMock.Verify(m => m.SetupJob(It.IsAny<TestJob>()), Times.Exactly(2));
        runner.Verify(m => m.Run(It.IsAny<TestJob>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [TestMethod]
    public void RunJobs_Exits_WhenEnableRunningJobsIsFalse()
    {
        var repo = CreateMockJobRepository();
        var jobs = new List<TestJob> { new TestJob { JobID = 1 }, new TestJob { JobID = 2 }, new TestJob { JobID = 3 } }; // Set up with 3 jobs
        var iter = jobs.GetEnumerator();
        repo.Setup(r => r.GetNextAvailable<TestJob>()).Returns(() => { iter.MoveNext(); return iter.Current; });
        var runner = CreateBackgroundJobRunner();
        var jobServiceMock = new Mock<JobService<TestJob>>(repo.Object, runner.Object);
        var jobService = jobServiceMock.Object;
        jobService.MaxConcurrentJobs = 5;
        jobService.EnableRunningJobs = false;

        // Test
        jobService.RunJobs();

        // Verify
        jobServiceMock.Verify(m => m.SetupJob(It.IsAny<TestJob>()), Times.Never);
        runner.Verify(m => m.Run(It.IsAny<TestJob>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    #endregion

    #region SubmitJobRequest
    [TestMethod]
    public void SubmitJobRequest_QueuesPrerequisiteJobs()
    {
        // Setup
        var job = new TestJob();
        var repo = CreateMockJobRepository();
        var runner = CreateBackgroundJobRunner();
        var jobServiceMock = new Mock<JobService<TestJob>>(repo.Object, runner.Object);
        var jobService = jobServiceMock.Object;
        jobService.EnableRunningJobs = true;

        // Test
        jobService.SubmitJobRequest(job);

        // Verify
        jobServiceMock.Verify(m => m.QueuePrerequisites(It.Is<TestJob>(j => j == job)), Times.Once());
    }

    [TestMethod]
    public void SubmitJobRequest_SetsJobToReady_WhenNoPrereqs()
    {
        // Setup
        var job = new TestJob();
        var repo = CreateMockJobRepository();
        var runner = CreateBackgroundJobRunner();
        var jobServiceMock = new Mock<JobService<TestJob>>(repo.Object, runner.Object);
        var jobService = jobServiceMock.Object;
        jobService.EnableRunningJobs = true;

        // Test
        jobService.SubmitJobRequest(job);

        // Verify
        Assert.AreEqual(JobStatus.Ready, job.Status);
    }

    //[TestMethod]
    //public void SubmitJobRequest_SetsJobToQueued_WhenPrereqsExist()
    //{
    //    // Setup
    //    var job = new TestJob();
    //    job.PrerequisiteJobs.Add(new TestJob());
    //    job.PrerequisiteJobs.Add(new TestJob());

    //    var repo = CreateMockJobRepository();
    //    var runner = CreateBackgroundJobRunner();
    //    var jobServiceMock = new Mock<JobService<TestJob>>(repo.Object, runner.Object);
    //    var jobService = jobServiceMock.Object;

    //    // Test
    //    jobService.SubmitJobRequest(job);

    //    // Verify
    //    Assert.AreEqual(JobStatus.Queued, job.Status);
    //}

    [TestMethod]
    public void SubmitJobRequest_AddsReadyJobToRepo_WhenNoPrereqs()
    {
        // Setup
        var job = new TestJob();

        var repo = CreateMockJobRepository();
        var runner = CreateBackgroundJobRunner();
        var jobServiceMock = new Mock<JobService<TestJob>>(repo.Object, runner.Object);
        var jobService = jobServiceMock.Object;
        jobService.EnableRunningJobs = true;

        // Test
        jobService.SubmitJobRequest(job);

        // Verify
        repo.Verify(m => m.Add(It.Is<TestJob>(j => j.Status == JobStatus.Ready)), Times.Once());
    }

    //[TestMethod]
    //public void SubmitJobRequest_AddsQueuedJobToRepo_WhenPrereqsExist()
    //{
    //    // Setup
    //    var job = new TestJob();
    //    job.PrerequisiteJobs.Add(new TestJob());
    //    job.PrerequisiteJobs.Add(new TestJob());

    //    var repo = CreateMockJobRepository();
    //    var runner = CreateBackgroundJobRunner();
    //    var jobServiceMock = new Mock<JobService<TestJob>>(repo.Object, runner.Object);
    //    var jobService = jobServiceMock.Object;

    //    // Test
    //    jobService.SubmitJobRequest(job);

    //    // Verify
    //    repo.Verify(m => m.Add(It.Is<TestJob>(j => j.Status == JobStatus.Queued)), Times.Once());
    //}
    #endregion

    #region FinishJob

    [TestMethod]
    public void FinishJob_RunsThreeTimes_WhenThreeJobsRun()
    {
        var repo = CreateMockJobRepository();
        var jobs = new List<TestJob> { new TestJob { JobID = 1 }, new TestJob { JobID = 2 }, new TestJob { JobID = 3 } }; // Set up with 3 jobs
        var iter = jobs.GetEnumerator();
        repo.Setup(r => r.GetNextAvailable<TestJob>()).Returns(() => { iter.MoveNext(); return iter.Current; });
        var runner = new BackgroundJobRunner<TestJob>();
        var jobServiceMock = new Mock<JobService<TestJob>>(repo.Object, runner);
        var jobService = jobServiceMock.Object;
        jobService.MaxConcurrentJobs = 5;
        jobService.EnableRunningJobs = true;

        // Test
        AsyncContext.Run(async () =>
        {
            await Task.WhenAll(jobService.RunJobs().ToArray());
        });


        // Verify
        jobServiceMock.Verify(m => m.FinishJob(It.IsAny<TestJob>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    #endregion

    #region RunningJobCount
    [TestMethod]
    public void RunningJobCount_IsZero_WhenThreeJobsFinished()
    {
        var repo = CreateMockJobRepository();
        var jobs = new List<TestJob> { new TestJob { JobID = 1 }, new TestJob { JobID = 2 }, new TestJob { JobID = 3 } }; // Set up with 3 jobs
        var iter = jobs.GetEnumerator();
        repo.Setup(r => r.GetNextAvailable<TestJob>()).Returns(() => { iter.MoveNext(); return iter.Current; });
        var runner = new BackgroundJobRunner<TestJob>();
        var jobServiceMock = new Mock<JobService<TestJob>>(repo.Object, runner)
        {
            CallBase = true
        };
        var jobService = jobServiceMock.Object;
        jobService.MaxConcurrentJobs = 5;
        jobService.EnableRunningJobs = true;

        // Test
        AsyncContext.Run(async () =>
        {
            await Task.WhenAll(jobService.RunJobs().ToArray());
        });


        // Verify
        Assert.AreEqual(0, jobService.RunningJobCount);
    }
    #endregion

    #region JobSubmitted Event

    [TestMethod]
    public void JobSubmittedEvent_IsCalledThreeTimes_WhenThreeJobsSubmittedAndCallbackRegistered()
    {
        var repo = CreateMockJobRepository();
        var jobs = new List<TestJob> { new TestJob(), new TestJob(), new TestJob() }; // Set up with 3 jobs
        var iter = jobs.GetEnumerator();
        var runner = CreateBackgroundJobRunner();
        var jobServiceMock = new Mock<JobService<TestJob>>(repo.Object, runner.Object)
        {
            CallBase = true
        };
        var jobService = jobServiceMock.Object;
        jobService.MaxConcurrentJobs = 5;
        jobService.EnableRunningJobs = true;

        int timesCalled = 0;
        jobService.JobChanged += (o, e) => { if (e.ChangeType == JobChangeEventArgs.JobChangeType.Submitted) timesCalled++; };

        // Test
        foreach (var job in jobs)
            jobService.SubmitJobRequest(job);


        // Verify
        Assert.AreEqual(3, timesCalled);
    }

    #endregion

    #region JobStarted Event

    [TestMethod]
    public void JobStartedEvent_IsCalledThreeTimes_WhenThreeJobsStartedAndCallbackRegistered()
    {
        var repo = CreateMockJobRepository();
        var jobs = new List<TestJob> { new TestJob { JobID = 1 }, new TestJob { JobID = 2 }, new TestJob { JobID = 3 } }; // Set up with 3 jobs
        var iter = jobs.GetEnumerator();
        repo.Setup(r => r.GetNextAvailable<TestJob>()).Returns(() => { iter.MoveNext(); return iter.Current; });
        var runner = new BackgroundJobRunner<TestJob>();
        var jobServiceMock = new Mock<JobService<TestJob>>(repo.Object, runner)
        {
            CallBase = true
        };
        var jobService = jobServiceMock.Object;
        jobService.MaxConcurrentJobs = 5;
        jobService.EnableRunningJobs = true;

        int timesCalled = 0;
        jobService.JobChanged += (o, e) => { if (e.ChangeType == JobChangeEventArgs.JobChangeType.Started) timesCalled++; };

        // Test
        AsyncContext.Run(async () =>
        {
            await Task.WhenAll(jobService.RunJobs().ToArray());
        });


        // Verify
        Assert.AreEqual(3, timesCalled);
    }

    #endregion

    #region JobFinished Event

    [TestMethod]
    public void JobFinishedEvent_IsCalledThreeTimes_WhenThreeJobsFinishedAndCallbackRegistered()
    {
        var repo = CreateMockJobRepository();
        var jobs = new List<TestJob> { new TestJob { JobID = 1 }, new TestJob { JobID = 2 }, new TestJob { JobID = 3 } }; // Set up with 3 jobs
        var iter = jobs.GetEnumerator();
        repo.Setup(r => r.GetNextAvailable<TestJob>()).Returns(() => { iter.MoveNext(); return iter.Current; });
        var runner = new BackgroundJobRunner<TestJob>();
        var jobServiceMock = new Mock<JobService<TestJob>>(repo.Object, runner)
        {
            CallBase = true
        };
        var jobService = jobServiceMock.Object;
        jobService.MaxConcurrentJobs = 5;
        jobService.EnableRunningJobs = true;

        int timesCalled = 0;
        jobService.JobChanged += (o, e) => { if (e.ChangeType == JobChangeEventArgs.JobChangeType.Finished) timesCalled++; };

        // Test
        AsyncContext.Run(async () =>
        {
            await Task.WhenAll(jobService.RunJobs().ToArray());
        });


        // Verify
        Assert.AreEqual(3, timesCalled);
    }

    #endregion
}
