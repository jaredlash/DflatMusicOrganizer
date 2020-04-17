using Dflat.Application.Models;
using Dflat.Application.Repositories;
using Dflat.Application.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Nito.AsyncEx;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dflat.Application.UnitTests
{
    [TestClass]
    public class BackgroundJobRunnerTests
    {

        [TestMethod]
        public void BackgroundWork_IsRunOnDifferentThread()
        {
            
            var job = new TestJob();
            var runner = new BackgroundJobRunner<TestJob>();
            Thread startingThread = Thread.CurrentThread;
            Thread backgroundThread = null;

            runner.BackgroundWork = (n) => backgroundThread = Thread.CurrentThread;
            runner.FinishWork = (n) => { };

            AsyncContext.Run(() =>
            {
                runner.Run(job);
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
            Thread backgroundThread = null;
            Thread finishThread = null;


            runner.BackgroundWork = (n) => backgroundThread = Thread.CurrentThread;
            runner.FinishWork = (n) => finishThread = Thread.CurrentThread;

            AsyncContext.Run(async () =>
            {
                await runner.Run(job);
            });

            Assert.IsNotNull(finishThread);
            Assert.AreEqual(startingThread.ManagedThreadId, finishThread.ManagedThreadId);
        }
    }
}
