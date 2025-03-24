using Dflat.Application.Models;
using Dflat.Application.Services.JobServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nito.AsyncEx;
using System.Threading;

namespace Dflat.Application.UnitTests.Services.JobServices;

[TestClass]
public class BackgroundJobRunnerTests
{

    [TestMethod]
    public void BackgroundWork_IsRunOnDifferentThread()
    {

        var job = new TestJob();
        var runner = new BackgroundJobRunner<TestJob>();
        Thread startingThread = Thread.CurrentThread;
        Thread? backgroundThread = null;

        runner.BackgroundWork = (n, t) => backgroundThread = Thread.CurrentThread;
        runner.FinishWork = (n, t) => { };

        AsyncContext.Run(() =>
        {
            runner.Run(job, new CancellationToken());
        }
        );

        Assert.IsNotNull(backgroundThread);
        Assert.AreNotEqual(startingThread.ManagedThreadId, backgroundThread.ManagedThreadId);
    }

    [TestMethod]
    public void FinishWork_IsRunOnCreatingThread()
    {
        var job = new TestJob();
        var runner = new BackgroundJobRunner<TestJob>();
        Thread startingThread = Thread.CurrentThread;
        Thread? backgroundThread = null;
        Thread? finishThread = null;


        runner.BackgroundWork = (n, t) => backgroundThread = Thread.CurrentThread;
        runner.FinishWork = (n, t) => finishThread = Thread.CurrentThread;

        AsyncContext.Run(async () =>
        {
            await runner.Run(job, new CancellationToken());
        });

        Assert.IsNotNull(finishThread);
        Assert.AreEqual(startingThread.ManagedThreadId, finishThread.ManagedThreadId);
    }

    [TestMethod]
    public void Run_CatchesExeptions_WhenExceptionsThrownInBackgroundWork()
    {
        var job = new TestJob();
        var runner = new BackgroundJobRunner<TestJob>();
        bool finishWorkIsCalled = false;

        runner.BackgroundWork = (n, t) => { throw new System.Exception(); };
        runner.FinishWork = (n, t) => { finishWorkIsCalled = true; };

        AsyncContext.Run(async () =>
        {
            await runner.Run(job, new CancellationToken());
        }
        );

        Assert.IsTrue(finishWorkIsCalled);
        Assert.IsFalse(string.IsNullOrEmpty(job.Errors));
        Assert.AreEqual(JobStatus.Error, job.Status);
    }
}
